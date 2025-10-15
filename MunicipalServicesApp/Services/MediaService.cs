using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MunicipalServicesApp.Services
{
    /// <summary>
    /// Centralised wrapper for OpenFileDialog with validation.
    /// </summary>
    public static class MediaService
    {
        // Whitelisted extensions. Validation is kept simple and local.
        private static readonly string[] AllowedExtensions = new[]
        {
            ".jpg", ".jpeg", ".png", ".pdf", ".docx", ".xlsx"
        };

        /// <summary>
        /// Pops an OpenFileDialog; if a valid file is chosen, returns true and the path via out parameter.
        /// </summary>
        public static bool TrySelectAttachment(IWin32Window owner, out string path)
        {
            path = null;

            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = "Select supporting media (optional)";
                ofd.Filter = "Images & Documents|*.jpg;*.jpeg;*.png;*.pdf;*.docx;*.xlsx";
                ofd.CheckFileExists = true;
                ofd.Multiselect = false;

                // Show dialog.
                if (ofd.ShowDialog(owner) == DialogResult.OK)
                {
                    var ext = Path.GetExtension(ofd.FileName)?.ToLowerInvariant();

                    // 1) Extension guard
                    if (!AllowedExtensions.Contains(ext))
                    {
                        MessageBox.Show(owner,
                            "Unsupported file type. Allowed: .jpg, .jpeg, .png, .pdf, .docx, .xlsx",
                            "Attachment Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    // 2) Existence guard
                    if (!File.Exists(ofd.FileName))
                    {
                        MessageBox.Show(owner,
                            "File no longer exists. Please select a valid file.",
                            "Attachment Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    // Return the chosen path.
                    path = ofd.FileName;
                    return true;
                }
            }

            // User cancelled or closed the dialog.
            return false;
        }
    }
}
