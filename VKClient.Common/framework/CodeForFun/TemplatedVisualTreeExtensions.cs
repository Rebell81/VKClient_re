using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace VKClient.Common.Framework.CodeForFun
{
  public static class TemplatedVisualTreeExtensions
  {
    public static T GetFirstLogicalChildByType<T>(this FrameworkElement parent, bool applyTemplates) where T : FrameworkElement
    {
      Queue<FrameworkElement> frameworkElementQueue = new Queue<FrameworkElement>();
      frameworkElementQueue.Enqueue(parent);
      while (frameworkElementQueue.Count > 0)
      {
        FrameworkElement frameworkElement = frameworkElementQueue.Dequeue();
        Control control = frameworkElement as Control;
        if (applyTemplates && control != null)
          control.ApplyTemplate();
        if (frameworkElement is T && frameworkElement != parent)
          return (T) frameworkElement;
        IEnumerator<FrameworkElement> enumerator = ((IEnumerable<FrameworkElement>) Enumerable.OfType<FrameworkElement>((IEnumerable) ((DependencyObject) frameworkElement).GetVisualChildren())).GetEnumerator();
        try
        {
          while (((IEnumerator) enumerator).MoveNext())
          {
            FrameworkElement current = enumerator.Current;
            frameworkElementQueue.Enqueue(current);
          }
        }
        finally
        {
          if (enumerator != null)
            ((IDisposable) enumerator).Dispose();
        }
      }
      return default (T);
    }

    public static IEnumerable<T> GetLogicalChildrenByType<T>(this FrameworkElement parent, bool applyTemplates) where T : FrameworkElement
    {
      if (applyTemplates && parent is Control)
        ((Control) parent).ApplyTemplate();
      Queue<FrameworkElement> queue = new Queue<FrameworkElement>((IEnumerable<FrameworkElement>) Enumerable.OfType<FrameworkElement>((IEnumerable) ((DependencyObject) parent).GetVisualChildren()));
      while (queue.Count > 0)
      {
        FrameworkElement element = queue.Dequeue();
        if (applyTemplates && element is Control)
          ((Control) element).ApplyTemplate();
        if (element is T)
          yield return (T) element;
        IEnumerator<FrameworkElement> enumerator = ((IEnumerable<FrameworkElement>) Enumerable.OfType<FrameworkElement>((IEnumerable) ((DependencyObject) element).GetVisualChildren())).GetEnumerator();
        try
        {
          while (((IEnumerator) enumerator).MoveNext())
            queue.Enqueue(enumerator.Current);
        }
        finally
        {
          if (enumerator != null)
            ((IDisposable) enumerator).Dispose();
        }
        element =  null;
      }
    }
  }
}
