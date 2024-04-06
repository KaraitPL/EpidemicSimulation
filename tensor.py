
from datetime import datetime
from os import system
import os
import tensorflow as tf
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
input_size = 9
output_size = 4
training_size = 142000

def get_data():
    data = pd.read_csv(r"Dane\sds.csv")
    training_size = int(0.8 * len(data))
    return data

model = tf.keras.Sequential()
model.add(tf.keras.layers.Dense(units=64, activation='relu', input_shape=(input_size,)))
model.add(tf.keras.layers.Dense(units=64, activation='relu'))
model.add(tf.keras.layers.Dense(units=output_size))

model.compile(optimizer='adam', loss='mean_squared_error')

data = get_data()
data1 = pd.DataFrame.to_numpy(data)
np.random.shuffle(data1)
X = data1[:training_size, :9]
y = data1[:training_size, 9:]
train_features = X
train_labels = y

history = model.fit(train_features, train_labels, epochs=200, batch_size=training_size, verbose=1)
#test_features = training_data = data.iloc[training_size:, :9]
test_features = data1[training_size:, :9]
test_labels = data1[training_size:, 9:]
#predictions = model.predict(test_features)
print("Testing:")
loss = model.evaluate(test_features, test_labels)

# print(predictions)
plt.plot(history.history['loss'])
plt.title("Wartość fukcji błędu - TensorFlow")
plt.ylabel("Wartość błędu")
plt.xlabel("Numer iteracji")
plt.yscale("log")
time = datetime.now()
plt.savefig("plots/" + datetime.now().strftime("%Y%m%d%H%M%S"))
plt.show()
os.system("pause")