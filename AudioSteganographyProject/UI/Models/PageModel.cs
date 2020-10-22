using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSteganographyProject.interfaces;
using AudioSteganographyProject.Interfaces;

namespace AudioSteganographyProject.UI.Models
{
    public abstract class PageModel : PageModelInterface
    {        public abstract void updateModel(object pageContent);
    }
}
