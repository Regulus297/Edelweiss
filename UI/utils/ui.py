from PyQt5.QtWidgets import QWidget


class UI:
    @staticmethod
    def close(widget: QWidget):
        for binding in getattr(widget, "__bindings__", []):
            binding.discard()
        for child in widget.children():
            if hasattr(child, "__bindings__"):
                UI.close(child)
        widget.deleteLater()