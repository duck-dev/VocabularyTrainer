using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Styling;
using VocabularyTrainer.Interfaces;
using VocabularyTrainer.Models;

namespace VocabularyTrainer.UtilityCollection;

public static partial class Utilities
{
    public delegate void NotifyItemAddedEventHandler(VocabularyItem item);
    public static event NotifyItemAddedEventHandler? NotifyItemAdded;
    
    public const string AssetsPath = "avares://VocabularyTrainer/Assets/";
        
    /// <summary>
    /// The parent path of all settings- and data-files
    /// </summary>
    public static string FilesParentPath
    {
        get
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create);
            string directory = Path.Combine(appData, "VocabularyTrainer");
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);
            return directory;
        }
    }
        
    /// <summary>
    /// Log a message to the console (for debugging purposes).
    /// </summary>
    /// <param name="message">The message to be logged as a string.</param>
    public static void Log(string? message)
    {
        Console.WriteLine(message);
        System.Diagnostics.Trace.WriteLine(message);
    }
        
    /// <summary>
    /// Add added/removed elements of a collection whose changes should be resettable to a list
    /// that is used to reset the changes if necessary.
    /// </summary>
    /// <param name="collection">The collection, which will keep track of all changes to the original collection
    /// (added/removed items), so it can be used to reset those changes.</param>
    /// <param name="args">Arguments of the CollectionChanged event of a <see cref="ObservableCollection{T}"/>
    /// with the changed (added/removed) items and other information about the changes.</param>
    /// <typeparam name="T">The type of the collection items.</typeparam>
    public static void AddChangedItems<T>(ICollection<T> collection, NotifyCollectionChangedEventArgs args) 
        where T : VocabularyItem, IContentVerification<T>
    {
        // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var newItems = args.NewItems?.Cast<T>();
                LoopItems(newItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                var oldItems = args.OldItems?.Cast<T>();
                LoopItems(oldItems);
                break;
        }

        void LoopItems(IEnumerable<T>? items)
        {
            if (items is null)
                return;
                
            foreach (var item in items)
            {
                item.ChangedAction = args.Action;
                NotifyItemAdded?.Invoke(item);
                if (CheckUnsavedContent(item, collection))
                    continue;
                    
                if (collection.Contains(item, ReferenceEqualityComparer.Instance))
                    collection.Remove(item);
                else
                    collection.Add(item);
            }
        }
    }

    /// <summary>
    /// Check if an item resolves an unsaved removal of a vocabulary item.
    /// </summary>
    /// <param name="item">The new item that could potentially resolve the unsaved removal.</param>
    /// <param name="collection">The collection that contains unsolved removals and additions.</param>
    /// <typeparam name="T">Type that implements <see cref="IContentVerification{T}"/></typeparam>
    /// <returns>Whether the item resolved the unsaved change or not.</returns>
    public static bool CheckUnsavedContent<T>(T item, ICollection<T> collection) where T : IContentVerification<T>
    {
        if (!item.MatchesUnsavedContent(collection, out T? identicalItem)) 
            return false;
            
        Lesson.CheckUnsavedEnabled = false; // Avoid StackOverflow by disabling check in `DataChanged` get-accessor
        item.SaveChanges();
        item.EqualizeChangedData();
        identicalItem?.SaveChanges();
        identicalItem?.EqualizeChangedData();
        Lesson.CheckUnsavedEnabled = true; // Back to normal

        collection.Remove(identicalItem!);
        collection.Remove(item);

        return true;
    }
        
    /// <summary>
    /// Retrieves a resource from a specified <see cref="IResourceNode"/> and tries to cast it to the specified type.
    /// </summary>
    /// <param name="element">The element to retrieve the resource from.</param>
    /// <param name="resourceName">The name of the resource you want to retrieve (key).</param>
    /// <typeparam name="T">The actual type of the resource you want to retrieve.</typeparam>
    /// <returns>The resource as it's actual type.</returns>
    public static T? GetResource<T>(IResourceNode element, string resourceName)
        where T : class
    {
        element.TryGetResource(resourceName, out object? resource);
        return resource as T;
    }

    /// <summary>
    /// Retrieves a resource from a specified <see cref="Style"/>, which is in turns contained
    /// in the <see cref="Styles"/> of an <see cref="IStyleHost"/>, and tries to cast it to the specified type.
    /// </summary>
    /// <param name="element">The element to retrieve the resource from.</param>
    /// <param name="resourceName">The name of the resource you want to retrieve (key).</param>
    /// <param name="styleIndex">The index of the <see cref="Style"/> inside the <see cref="Styles"/> collection
    /// of the <paramref name="element"/>.</param>
    /// <typeparam name="TResource">The actual type of the resource you want to retrieve.</typeparam>
    /// <typeparam name="TElement">The type of the element to retrieve the resource from.
    /// This type must implement <see cref="IResourceNode"/>.</typeparam>
    /// <returns>The resource as it's actual type.</returns>
    public static TResource? GetResourceFromStyle<TResource, TElement>(TElement? element, string resourceName, int styleIndex)
        where TResource : class
        where TElement : IStyleHost, IResourceNode
    {
        var styleInclude = element?.Styles[styleIndex] as StyleInclude;
        return (styleInclude?.Loaded is Style style ? GetResource<TResource>(style, resourceName) : null);
    }
    
    /// <summary>
    /// Creates a <see cref="Bitmap"/> image, based on an image-file whose path is specified as a passed argument.
    /// </summary>
    /// <param name="path">The path of the image-file.</param>
    /// <returns>The <see cref="Bitmap"/> image or null if the operation was successful due to a wrong path for example.</returns>
    public static Bitmap? CreateImage(string path)
    {
        var uri = new Uri(path);
        Stream? asset = null;
        try
        {
            IAssetLoader? assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
            asset = assets?.Open(uri);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return asset is null ? null : new Bitmap(asset);
    }
}