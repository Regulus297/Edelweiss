from interop import InteropMethod, SyncableProperty
from .widget_binding import WidgetBinding


class LocalizedBinding(WidgetBinding):
    get_method = None
    current_language = None

    def __init__(self, widget, data, name, **subscribers):
        if LocalizedBinding.get_method is None:
            LocalizedBinding.get_method = InteropMethod("Edelweiss:MainInterop.GetLocalization")
        if LocalizedBinding.current_language is None:
            LocalizedBinding.current_language = SyncableProperty("Edelweiss.CurrentLanguage")

        super().__init__(widget, data, name, **subscribers)
        self.subscribers = subscribers

        if self.bindable:
            return

        if self.prop.startswith("@"):
            self.prop = self.prop[1:]
            self._sync_non_bindable(**subscribers)
            return

        self._default = self.prop
        if ":" in self.prop:
            self.prop, self._default = self.prop.split(":")

        LocalizedBinding.current_language.ValueChanged += lambda _: self._update_value()
        self._update_value()


    def _update_value(self):
        temp = self.prop
        self.prop = LocalizedBinding.get_method(self.prop, self.default)
        self._sync_non_bindable(**self.subscribers)
        self.prop = temp

    @property
    def default(self):
        return self._default

    @default.setter
    def default(self, value):
        self._default = value
        self._update_value()