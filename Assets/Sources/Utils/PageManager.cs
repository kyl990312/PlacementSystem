using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Page
{

}

public class PageSettings
{
    private Dictionary<Page, string> _pages = new Dictionary<Page, string>();

    public bool GetPage(Page pageID, out PageBase page)
    {
        page = null;
        if (!_pages.TryGetValue(pageID, out var path))
        {
            return false;
        }
        if (!ResourcesLoader.Load(path, out GameObject gObj))
        {
            return false;
        }
        if (!gObj.TryGetComponent(out page))
        {
            return false; 
        }
        return page != null;
    }
}

public class PageManager
{
    private PageSettings _pageSettings  = new PageSettings();
    private List<PageBase> _pages = new List<PageBase>();

    public void OpenPage(Page pageID)
    {
        if (!_pageSettings.GetPage(pageID, out var page))
            return;
        _pages.Add(page);
        page.Open();
    }

    public void Back()
    {
        if(_pages.Count <= 0)
            return;
        var last = _pages[_pages.Count - 1];
        ClosePage(last);
    }

    public void ClosePage(PageBase page)
    {
        page.Close();
        int index = _pages.FindIndex(Equals);
        if (index < 0) return;

        _pages.RemoveAt(index);
    }

    private bool Equals(PageBase a, PageBase b)
    {
        return ReferenceEquals(a, b);
    }
}
