using System.Windows.Automation;
using System.Threading;


namespace AutomationFramework
{
    public class Mouse
    {
        bool _isEnabled;
        int _timeout = 1;
        public void ClickControl(AutomationElement element)
        {
            try
            {
                object objPattern;
                InvokePattern invPattern = null;
                if (true == element.TryGetCurrentPattern(InvokePattern.Pattern, out objPattern))
                {
                    invPattern = objPattern as InvokePattern;
                }

                if (invPattern != null)
                {
                    invPattern.Invoke();
                    _isEnabled = true;

                    return;
                }

                SelectionItemPattern selPattern = null;
                if (element.TryGetCurrentPattern(SelectionItemPattern.Pattern, out objPattern))
                {
                    selPattern = objPattern as SelectionItemPattern;
                }
                if (selPattern != null)
                {
                    selPattern.Select();
                    _isEnabled = true;
                    return;
                    
                }
            }

            catch (ElementNotEnabledException)
            {
                _isEnabled = false;
                System.Console.WriteLine("Element is not enabled.");

               
                if (_isEnabled == false && _timeout <= 1000)
                {
                    _timeout++;
                    System.Console.WriteLine("Attempt {0}", _timeout);
                    ClickControl(element);
                }

                if(_isEnabled == false && _timeout >= 2000)
                {
                    System.Console.WriteLine("Timed out trying to click element");
                    throw new NoClickablePointException("ClickControl could not click due to a disabled element");
                }
            }






        }

        public void ClickDropDown(AutomationElement ExpandCollapseItem, bool Expand = true)
        {
            Thread.Sleep(1000);
            ExpandCollapsePattern expandCollapsePattern = ExpandCollapseItem.GetCurrentPattern(ExpandCollapsePattern.Pattern) as ExpandCollapsePattern;

            if (expandCollapsePattern != null)
            {
                if (Expand)
                {
                    expandCollapsePattern.Expand();
                }
                else
                {
                    expandCollapsePattern.Collapse();
                }
            }
        }
    }
}
