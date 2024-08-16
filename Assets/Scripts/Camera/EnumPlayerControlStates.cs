using System;

[Flags]
public enum PlayerControlStates
{
    None                 = 0,
    FrameStretching      = 1 << 0,
    MouseMove            = 1 << 1,
    PresedCtrl           = 1 << 2,
    HoldLeftMouseButton  = 1 << 3,
}