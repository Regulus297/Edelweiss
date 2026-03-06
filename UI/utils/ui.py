from PyQt5.QtWidgets import QWidget


class UI:
    @staticmethod
    def close(widget: QWidget):
        for child in widget.children():
            if isinstance(child, QWidget):
                UI.close(child)
                for binding in getattr(child, "__bindings__", []):
                    binding.discard()
        widget.close()