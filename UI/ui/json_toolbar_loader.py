import json
from copy import copy
from typing import Union

from PyQt5.QtWidgets import QToolBar, QAction, QMenu, QWidgetAction, QToolButton

from network import PyNetworkManager
from plugins import get_extra_data_safe
from .json_widget_loader import JSONWidgetLoader
from .main_window import MappingWindow
from plugins import get_event_data


class JSONToolbarLoader:
    @staticmethod
    def init_toolbar(toolbar: Union[QMenu, QToolBar], data: dict, clear = lambda toolbar: toolbar.clear()):
        clear(toolbar)
        for item in data["items"]:
            action = None
            if item["type"] == "action":
                action = QAction(toolbar)
                action.setText(str(item["name"]))

                if "shortcut" in item:
                    action.setShortcut(item["shortcut"])

                action_id = str(item["id"])
                if hasattr(toolbar, "json_loader_name"):
                    action_id = getattr(toolbar, "json_loader_name") + "/" + action_id
                setattr(action, "json_loader_name", action_id)

                if "onclick" in item.keys():
                    JSONToolbarLoader.register_onclick(action, item)

            elif item["type"] == "menu":
                menu = QMenu(toolbar)
                action_id = item["id"]
                if hasattr(toolbar, "json_loader_name"):
                    action_id = getattr(toolbar, "json_loader_name") + "/" + action_id
                setattr(menu, "json_loader_name", action_id)
                JSONToolbarLoader.init_toolbar(menu, item)

                action = QAction(toolbar)
                action.setText(item["name"])
                action.setMenu(menu)
            elif item["type"] == "widget":
                if toolbar is QMenu:
                    print("Putting widgets inside menus is not supported")
                    continue
                toolbar.addWidget(JSONWidgetLoader.init_widget(item["data"], toolbar))
                continue

            if action is None:
                print(f"Unrecognized item type {item['type']}")
                return

            toolbar.addAction(action)

    @staticmethod
    def register_onclick(action, data):
        netcode, extraData = get_event_data(data["onclick"])
        action.triggered.connect(lambda: PyNetworkManager.send_packet(netcode, json.dumps({
            "tab": MappingWindow.instance.current_tab,
            "name": getattr(action, "json_loader_name"),
            "extraData": extraData
        })))