from network import PacketReceiver
from plugins import plugin_loadable, load_dependencies, JSONPreprocessor
from ui import MappingWindow, CustomDrawItem
from Edelweiss.Network import Netcode


@load_dependencies("../widgets/zoomable_view.py")
@plugin_loadable
class ModifyItemShapeReceiver(PacketReceiver):
    def __init__(self):
        super().__init__(Netcode.MODIFY_ITEM)

    def process_packet(self, packet):
        data = JSONPreprocessor.loads(packet.data)
        widget = MappingWindow.instance.get_tracked_widget(data["widget"])
        if type(widget) != ZoomableView:
            print(f"Failed to add item as widget {type(widget)} is not a {ZoomableView}")
            return
        
        if data["item"] not in widget.trackedItems.keys():
            print(f"No item found with name {data['item']}")
            return
        
        item = widget.trackedItems[data["item"]]
        gItem = widget.trackedGraphicsItems[data["item"]]
        if "action" not in data or data["action"] == "modify":
            widget.trackedItems[data["item"]].update(data["data"])
            widget.trackedGraphicsItems[data["item"]].refresh(data["data"], "shapes" in data["data"])
        elif data["action"] == "remove":
            indices = [data["index"]] if isinstance(data["index"], int) else data["index"]
            for index in reversed(sorted(indices)):
                del item["shapes"][index]
                del gItem.shapeRenderers[index]
                gItem.refresh({})
        elif data["action"] == "add":
            for shape in data["shapes"]:
                gItem.addShape(shape)
            gItem.refresh({})
        elif data["action"] == "clear":
            for child in gItem.childItems():
                gItem.scene().removeItem(child)
                del child
