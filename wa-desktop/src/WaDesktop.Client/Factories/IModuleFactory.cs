namespace WaDesktop.Client.Factories
{
    /// <summary>
    /// Merakit sebuah modul tab (View + Presenter + DI Scope) berdasarkan moduleKey.
    /// Detail perakitan & lifetime management disembunyikan dari ShellPresenter.
    /// </summary>
    public interface IModuleFactory
    {
        /// <summary>
        /// Membuat instance modul untuk satu tab Workspace.
        /// Panggil Dispose() pada ModuleInstance saat tab ditutup.
        /// </summary>
        ModuleInstance Create(string moduleKey);
    }
}
