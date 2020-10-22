using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSteganographyProject.interfaces
{
    /// <summary>
    /// Interface implemented by all data models for pages.
    /// </summary>
    interface PageModelInterface
    {
        void updateModel(object pageContent);
    }
}
