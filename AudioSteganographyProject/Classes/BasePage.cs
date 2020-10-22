using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AudioSteganographyProject.Classes
{
    /// <summary>
    /// Abstract class extending Page, used to require common functionality from child pages.
    /// </summary>
    abstract public class BasePage : Page
    {
        abstract public ContentControl getContentControl();
    }
}
