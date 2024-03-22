from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional

DESCRIPTOR: _descriptor.FileDescriptor

class InputPosition(_message.Message):
    __slots__ = ("x", "z")
    X_FIELD_NUMBER: _ClassVar[int]
    Z_FIELD_NUMBER: _ClassVar[int]
    x: float
    z: float
    def __init__(self, x: _Optional[float] = ..., z: _Optional[float] = ...) -> None: ...

class OutputAI(_message.Message):
    __slots__ = ("fx", "fz")
    FX_FIELD_NUMBER: _ClassVar[int]
    FZ_FIELD_NUMBER: _ClassVar[int]
    fx: float
    fz: float
    def __init__(self, fx: _Optional[float] = ..., fz: _Optional[float] = ...) -> None: ...
