from network import PacketReceiver
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor
from ui import MappingWindow, JSONWidgetLoader
from Edelweiss.Network import Netcode


@load_dependencies("../widgets/zoomable_view.py")
@plugin_loadable
class RefreshWidgetReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.REFRESH_WIDGETS)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        for widgetTracker in data["widgets"]:
            widget = MappingWindow.instance.get_tracked_widget(widgetTracker)
            if widget is not None:
                data = getattr(widget, "__json_data__")
                JSONWidgetLoader.widget_creators[data["type"]].refresh_widget(widget)
