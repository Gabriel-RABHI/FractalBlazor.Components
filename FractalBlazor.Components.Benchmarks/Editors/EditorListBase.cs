using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Benchmarks.Editors
{
    /// <summary>
    /// Root of a benchmarked stack of editors, one per applicant, which can be re-rendered
    /// from the benchmark through <see cref="Current"/>.
    /// </summary>
    public abstract class EditorListBase : ComponentBase
    {
        public static EditorListBase? Current { get; private set; }

        [Parameter]
        public IReadOnlyList<ApplicantDto> Applicants { get; set; } = [];

        protected override void OnInitialized() => Current = this;

        /// <summary>
        /// Moves every applicant to the next status and toggles its check box, so each editor
        /// renders again with new values.
        /// </summary>
        public Task BumpAsync() => InvokeAsync(() =>
        {
            foreach (var applicant in Applicants)
            {
                applicant.Status = (ApplicantStatus)(((int)applicant.Status + 1) % ApplicantDto.Statuses.Count);
                applicant.Interviewed = !applicant.Interviewed;
            }
            StateHasChanged();
        });
    }
}
