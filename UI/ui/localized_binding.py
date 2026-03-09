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

        if self.bindable:
            return

        if self.prop.startswith("@"):
            self.prop = self.prop[1:]
            self._sync_non_bindable(**subscribers)
            return

        self.default = None
        if ":" in self.prop:
            self.prop, self.default = self.prop.split(":")

        LocalizedBinding.current_language.ValueChanged += lambda _: self._update_value(**subscribers)
        self._update_value(**subscribers)


    def _update_value(self, **subscribers):
        temp = self.prop
        self.prop = LocalizedBinding.get_method(self.prop, self.default)
        self._sync_non_bindable(**subscribers)
        self.prop = temp
