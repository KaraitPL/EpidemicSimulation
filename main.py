from os import system
import os
import pandas as pd
import numpy as np
input_size = 9
output_size = 4
training_size = 126000

def get_data():
    data = pd.read_csv(r'Dane\daneLong.csv')
    return data


def forward_propagation(X, parameters):
    W1, b1, W2, b2, W3, b3 = parameters["W1"], parameters["b1"], parameters["W2"], parameters["b2"], parameters["W3"], parameters["b3"]
    Z1 = np.dot(W1, X) + b1
    A1 = np.maximum(0, Z1)
    Z2 = np.dot(W2, A1) + b2
    A2 = np.maximum(0, Z2)
    Z3 = np.dot(W3, A2) + b3
    return Z3

def initialize_parameters(input_size, hidden_size1, hidden_size2, output_size):
    W1 = np.random.randn(hidden_size1, input_size) * 0.01
    b1 = np.zeros((hidden_size1, 1))
    W2 = np.random.randn(hidden_size2, hidden_size1) * 0.01
    b2 = np.zeros((hidden_size2, 1))
    W3 = np.random.randn(output_size, hidden_size2) * 0.01
    b3 = np.zeros((output_size, 1))
    return {"W1": W1, "b1": b1, "W2": W2, "b2": b2, "W3": W3, "b3": b3}

def initialize_adam(parameters):
    L = len(parameters) // 2
    v = {}
    s = {}

    for l in range(L):
        v["dW" + str(l+1)] = np.zeros_like(parameters["W" + str(l+1)])
        v["db" + str(l+1)] = np.zeros_like(parameters["b" + str(l+1)])
        s["dW" + str(l+1)] = np.zeros_like(parameters["W" + str(l+1)])
        s["db" + str(l+1)] = np.zeros_like(parameters["b" + str(l+1)])

    return v, s

def update_parameters_with_adam(parameters, grads, v, s, t, learning_rate, beta1=0.9, beta2=0.999, epsilon=1e-8):
    L = len(parameters) // 2
    v_corrected = {}
    s_corrected = {}

    for l in range(L):
        v["dW" + str(l+1)] = beta1 * v["dW" + str(l+1)] + (1 - beta1) * grads["dW" + str(l+1)]
        v["db" + str(l+1)] = beta1 * v["db" + str(l+1)] + (1 - beta1) * grads["db" + str(l+1)]
        v_corrected["dW" + str(l+1)] = v["dW" + str(l+1)] / (1 - beta1**t)
        v_corrected["db" + str(l+1)] = v["db" + str(l+1)] / (1 - beta1**t)

        s["dW" + str(l+1)] = beta2 * s["dW" + str(l+1)] + (1 - beta2) * (grads["dW" + str(l+1)]**2)
        s["db" + str(l+1)] = beta2 * s["db" + str(l+1)] + (1 - beta2) * (grads["db" + str(l+1)]**2)
        s_corrected["dW" + str(l+1)] = s["dW" + str(l+1)] / (1 - beta2**t)
        s_corrected["db" + str(l+1)] = s["db" + str(l+1)] / (1 - beta2**t)

        parameters["W" + str(l+1)] -= learning_rate * v_corrected["dW" + str(l+1)] / (np.sqrt(s_corrected["dW" + str(l+1)]) + epsilon)
        parameters["b" + str(l+1)] -= learning_rate * v_corrected["db" + str(l+1)] / (np.sqrt(s_corrected["db" + str(l+1)]) + epsilon)

    return parameters, v, s

def train_model(X, Y, hidden_size1, hidden_size2, num_iterations, learning_rate):
    input_size = X.shape[1]
    output_size = Y.shape[1]

    parameters = initialize_parameters(input_size, hidden_size1, hidden_size2, output_size)
    v, s = initialize_adam(parameters)
    t = 0

    for i in range(num_iterations):
        # Propagacja do przodu
        Z1 = np.dot(parameters["W1"], X.T) + parameters["b1"]
        A1 = np.maximum(0, Z1)
        Z2 = np.dot(parameters["W2"], A1) + parameters["b2"]
        A2 = np.maximum(0, Z2)
        Z3 = np.dot(parameters["W3"], A2) + parameters["b3"]

        # Obliczanie funkcji kosztu (błąd średniokwadratowy)
        loss = np.mean((Z3 - Y.T)**2)

        # Propagacja wsteczna
        dZ3 = 2 * (Z3 - Y.T)
        dW3 = np.dot(dZ3, A2.T) / input_size
        db3 = np.mean(dZ3, axis=1, keepdims=True)
        dA2 = np.dot(parameters["W3"].T, dZ3)
        dZ2 = dA2 * (Z2 > 0)
        dW2 = np.dot(dZ2, A1.T) / input_size
        db2 = np.mean(dZ2, axis=1, keepdims=True)
        dA1 = np.dot(parameters["W2"].T, dZ2)
        dZ1 = dA1 * (Z1 > 0)
        dW1 = np.dot(dZ1, X) / input_size
        db1 = np.mean(dZ1, axis=1, keepdims=True)

        grads = {"dW1": dW1, "db1": db1, "dW2": dW2, "db2": db2, "dW3": dW3, "db3": db3}

        t += 1
        parameters, v, s = update_parameters_with_adam(parameters, grads, v, s, t, learning_rate)

        if i % 10 == 0:
            print(f"Iteration: {i}, Loss: {loss}")

    return parameters



data = get_data()
data1 = data.to_numpy()
np.random.shuffle(data1)

X = data1[:training_size, :input_size]
y = data1[:training_size, input_size:]

train_features = X
train_labels = y

parameters = train_model(train_features, train_labels, hidden_size1=32, hidden_size2=32, num_iterations=200, learning_rate=0.05)


test_features = data1[training_size:, :input_size]

predictions = forward_propagation(test_features.T, parameters)

print(predictions)
os.system("pause")