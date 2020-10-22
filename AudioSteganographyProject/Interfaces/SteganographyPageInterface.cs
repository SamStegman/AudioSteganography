using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSteganographyProject.UI.Models;

namespace AudioSteganographyProject.Interfaces
{
    /// <summary>
    /// Interface implemented by all Page subclasses.
    /// </summary>
    interface SteganographyPageInterface
    {
        PageModel getModel();

        void updateModel();
    }
}
