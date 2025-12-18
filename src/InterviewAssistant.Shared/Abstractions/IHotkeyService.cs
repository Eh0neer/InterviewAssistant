using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewAssistant.Shared.Abstractions
{
    public interface IHotkeyService
    {
        void Start();
        void Stop();

        event Action AltPressed;
        event Action AltReleased;

        event Action LeftMouseDown;
    }
}
