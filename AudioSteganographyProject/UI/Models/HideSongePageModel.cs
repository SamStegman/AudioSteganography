using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioSteganographyProject.UI.Views;

namespace AudioSteganographyProject.UI.Models
{
    public class HideSongPageModel : PageModel
    {
        public String songPath, fileToBeHiddenPath, combinedDestinationPath;
        public HideSongPageModel()
        {
            this.songPath = "";
            this.fileToBeHiddenPath = "";
            this.combinedDestinationPath = "";
        }

        public override void updateModel(object pageContent)
        {
            this.songPath = ((HideSongPage)pageContent).SongFile.FilePathString;
            this.fileToBeHiddenPath = ((HideSongPage)pageContent).HiddenFile.FilePathString;
            this.combinedDestinationPath = ((HideSongPage)pageContent).NewFilePath.FilePathString;
        }
    }
}
