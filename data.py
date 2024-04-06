import pandas as pd
def get_data():
    data = pd.read_csv(r'Dane\daneLong.csv')
    return data