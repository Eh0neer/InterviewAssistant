using InterviewAssistant.Shared.Abstractions;
using InterviewAssistant.Win32.Hotkeys;
using InterviewAssistant.Win32.Overlay;
using Microsoft.Extensions.DependencyInjection;

namespace InterviewAssistant.Win32
{
    public static class Win32Module
    {
        public static IServiceCollection AddWin32(this IServiceCollection services)
        {
            services.AddSingleton<IOverlayService, Win32OverlayService>();
            services.AddSingleton<IHotkeyService, Win32HotkeyService>();
            return services;
        }
    }
}
