def get_extra_data_safe(data):
    if "extraData" not in data.keys():
        return {}

    return {key: str(value) for key, value in data["extraData"].items()}
