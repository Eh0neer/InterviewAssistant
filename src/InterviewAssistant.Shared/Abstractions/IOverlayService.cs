using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterviewAssistant.Shared.Abstractions
{
    public interface IOverlayService
    {
        void Initialize(IntPtr windowHandle);
        void Show();
        void Hide();

        void EnableInteraction();
        void DisableInteraction();

        void BeginDrag();
        void TryBeginDrag();
    }
}
