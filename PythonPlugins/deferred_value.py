class DeferredValue:
    def __init__(self, func):
        self._func = func
    
    def __call__(self, *args, **kwds):
        return self._func()(*args, **kwds)
    
    def __repr__(self):
        return repr(self._func())
    
    def __str__(self):
        return str(self._func())
    
    def __eq__(self, value):
        return self._func() == (value._func() if isinstance(value, DeferredValue) else value)
    
    def __lt__(self, value):
        return self._func() < value
    
    def __gt__(self, value):
        return self._func() > value
    
    def __le__(self, value):
        return self._func() <= value
    
    def __ge__(self, value):
        return self._func() >= value
    

    
    def __float__(self):
        return float(self._func())
    
    def __int__(self):
        return int(self._func())
    
    def __bool__(self):
        return bool(self._func())
    
    def __len__(self):
        return len(self._func())
    
    def __add__(self, value):
        return self._func() + value
    
    def __radd__(self, value):
        return value + self._func()
    
    def __sub__(self, value):
        return self._func() - value
    
    def __rsub__(self, value):
        return value - self._func()
    
    def __mul__(self, value):
        return self._func() * value
    
    def __rmul__(self, value):
        return value * self._func()
    
    def __truediv__(self, value):
        return self._func() / value
    
    def __rtruediv__(self, value):
        return value / self._func()
    
    def __mod__(self, value):
        return self._func() % value
    
    def __rmod__(self, value):
        return value % self._func()
    
    def __floordiv__(self, value):
        return self._func() // value
    
    def __rfloordiv__(self, value):
        return value // self._func()
    
    def __pow__(self, value):
        return self._func() ** value
    
    def __rpow__(self, value):
        return value ** self._func()
    
    def __matmul__(self, value):
        return self._func() @ value
    
    def __rmatmul__(self, value):
        return value @ self._func()
    

    def __and__(self, value):
        return self._func() & value
    
    def __rand__(self, value):
        return value & self._func()
    
    def __or__(self, value):
        return self._func() | value
    
    def __ror__(self, value):
        return value | self._func()
    
    def __xor__(self, value):
        return self._func() ^ value
    
    def __rxor__(self, value):
        return value ^ self._func()
    
    def __rshift__(self, value):
        return self._func() >> value
    
    def __rrshift__(self, value):
        return value >> self._func()
    
    def __lshift__(self, value):
        return self._func() << value
    
    def __rlshift__(self, value):
        return value << self._func()
    
    def __neg__(self):
        return -self._func()
    
    def __pos__(self):
        return +self._func()
    
    def __invert__(self):
        return ~self._func()
    
    def __iter__(self):
        yield from self._func()

    def __instancecheck__(self, x):
        return isinstance(self._func(), x)