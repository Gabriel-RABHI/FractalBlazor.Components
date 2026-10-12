using System.Text.Json;
using Microsoft.JSInterop;

namespace FractalBlazor.WebSite.ThemeEditor
{
    /// <summary>
    /// Keeps the theme editor in the browser (<c>localStorage</c>), on the server host as in
    /// WebAssembly: a reload, a lost circuit or a restart never loses a setting. The editor
    /// restores the session when it opens, then saves after every render that changed something.
    /// </summary>
    public sealed class ThemeEditorAutoSave(ThemeEditorModel model, IJSRuntime js)
    {
        private const string Key = "fb-theme-editor";

        private bool _restored;
        private string? _saved;

        /// <summary>
        /// Comes back to the saved session, once per editor model. Until then nothing is saved,
        /// so a fresh editor never overwrites the session it is about to restore. Returns
        /// <c>true</c> when a session was restored, and the editor must render again.
        /// </summary>
        public async Task<bool> RestoreAsync()
        {
            if (_restored)
                return false;

            try
            {
                var json = await js.InvokeAsync<string?>("fbWebSite.loadText", Key);
                if (json is not null)
                {
                    var saved = JsonSerializer.Deserialize(json, ThemeEditorJson.Default.ThemeEditorSnapshot);
                    if (saved?.Format == ThemeEditorSnapshot.CurrentFormat)
                    {
                        model.Restore(saved);
                        _saved = json;
                        _restored = true;
                        return true;
                    }
                }
                _restored = true;
            }
            catch (JsonException)
            {
                // A damaged session is replaced by the next save.
                _restored = true;
            }
            catch (Exception exception) when (exception is InvalidOperationException or JSException or JSDisconnectedException)
            {
                // No interactive JS yet (prerender) or any more: the next render tries again.
            }

            return false;
        }

        /// <summary>
        /// Saves the editor when it differs from the last saved state.
        /// </summary>
        public async Task SaveAsync()
        {
            if (!_restored)
                return;

            var json = JsonSerializer.Serialize(model.Capture(), ThemeEditorJson.Default.ThemeEditorSnapshot);
            if (json == _saved)
                return;

            try
            {
                await js.InvokeVoidAsync("fbWebSite.saveText", Key, json);
                _saved = json;
            }
            catch (Exception exception) when (exception is InvalidOperationException or JSException or JSDisconnectedException)
            {
                // Tried again after the next change.
            }
        }
    }
}
