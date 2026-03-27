from utils import CSUtils
from .dereference_node import DereferenceNode
from .indexer_node import IndexerNode
from .syncable_node import SyncableNode


class NodeParser:
    @staticmethod
    def parse(text: str, sync = True):
        parts = text.split(".")
        if len(parts) == 1:
            return SyncableNode(parts[0])

        prop = parts[-1]
        sub_node = NodeParser.parse(".".join(parts[:-1]), sync)

        if prop.startswith("@"):
            if CSUtils.typeIsBindableList(type(sub_node.get())):
                return IndexerNode(sub_node, int(prop[1:]), sync)
            return IndexerNode(sub_node, prop[1:], sync)

        return DereferenceNode(sub_node, prop, sync)
