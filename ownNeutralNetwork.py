from datetime import datetime
from os import system
import os
import pandas as pd
import numpy as np
import pickle
import matplotlib.pyplot as plt

input_size = 9
output_size = 4
training_size = 134400 


def get_data():
    data = pd.read_csv(r"Dane\bigData - const seed.csv")
    training_size = int(0.8 * len(data))
    return data


def ReLu(Z):
    return np.maximum(0, Z)


def ReLu_derivative(dA, Z):
    return dA * (Z > 0)


def sigmoid(Z):
    return np.maximum(0, Z)
    #return 1 / (1 + np.exp(-Z))


def sigmoid_derivative(dA, Z):
    return dA * (Z > 0)
    # A = sigmoid(Z)
    # return dA * A * (1 - A)


def forward_propagation(X, parameters, actFun=ReLu):
    Z1 = np.dot(parameters["W1"], X.T) + parameters["b1"]
    A1 = actFun(Z1)
    Z2 = np.dot(parameters["W2"], A1) + parameters["b2"]
    A2 = actFun(Z2)
    Z3 = np.dot(parameters["W3"], A2) + parameters["b3"]
    return Z1, A1, Z2, A2, ReLu(Z3)


def backward_propagation(X, Y, Z1, A1, Z2, A2, Z3, parameters, actFunDer):
    dZ3 = 2 * (Z3 - Y.T)
    dW3 = np.dot(dZ3, A2.T) / input_size
    db3 = np.mean(dZ3, axis=1, keepdims=True)
    dA2 = np.dot(parameters["W3"].T, dZ3)
    dZ2 = actFunDer(dA2, Z2)
    dW2 = np.dot(dZ2, A1.T) / input_size
    db2 = np.mean(dZ2, axis=1, keepdims=True)
    dA1 = np.dot(parameters["W2"].T, dZ2)
    dZ1 = actFunDer(dA1, Z1)
    dW1 = np.dot(dZ1, X) / input_size
    db1 = np.mean(dZ1, axis=1, keepdims=True)

    grads = {"dW1": dW1, "db1": db1, "dW2": dW2, "db2": db2, "dW3": dW3, "db3": db3}
    return grads


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
        v["dW" + str(l + 1)] = np.zeros_like(parameters["W" + str(l + 1)])
        v["db" + str(l + 1)] = np.zeros_like(parameters["b" + str(l + 1)])
        s["dW" + str(l + 1)] = np.zeros_like(parameters["W" + str(l + 1)])
        s["db" + str(l + 1)] = np.zeros_like(parameters["b" + str(l + 1)])

    return v, s


def update_parameters_with_adam(
    parameters, grads, v, s, t, learning_rate, beta1=0.9, beta2=0.999, epsilon=1e-8
):
    L = len(parameters) // 2
    v_corrected = {}
    s_corrected = {}

    for l in range(L):
        v["dW" + str(l + 1)] = (
            beta1 * v["dW" + str(l + 1)] + (1 - beta1) * grads["dW" + str(l + 1)]
        )
        v["db" + str(l + 1)] = (
            beta1 * v["db" + str(l + 1)] + (1 - beta1) * grads["db" + str(l + 1)]
        )
        v_corrected["dW" + str(l + 1)] = v["dW" + str(l + 1)] / (1 - beta1**t)
        v_corrected["db" + str(l + 1)] = v["db" + str(l + 1)] / (1 - beta1**t)

        s["dW" + str(l + 1)] = beta2 * s["dW" + str(l + 1)] + (1 - beta2) * (
            grads["dW" + str(l + 1)] ** 2
        )
        s["db" + str(l + 1)] = beta2 * s["db" + str(l + 1)] + (1 - beta2) * (
            grads["db" + str(l + 1)] ** 2
        )
        s_corrected["dW" + str(l + 1)] = s["dW" + str(l + 1)] / (1 - beta2**t)
        s_corrected["db" + str(l + 1)] = s["db" + str(l + 1)] / (1 - beta2**t)

        parameters["W" + str(l + 1)] -= (
            learning_rate
            * v_corrected["dW" + str(l + 1)]
            / (np.sqrt(s_corrected["dW" + str(l + 1)]) + epsilon)
        )
        parameters["b" + str(l + 1)] -= (
            learning_rate
            * v_corrected["db" + str(l + 1)]
            / (np.sqrt(s_corrected["db" + str(l + 1)]) + epsilon)
        )

    return parameters, v, s


def train_model(
    X,
    Y,
    hidden_size1,
    hidden_size2,
    num_iterations,
    learning_rate,
    actFun=ReLu,
    actFunDer=ReLu_derivative,
):
    input_size = X.shape[1]
    output_size = Y.shape[1]

    parameters = initialize_parameters(
        input_size, hidden_size1, hidden_size2, output_size
    )
    v, s = initialize_adam(parameters)
    t = 0
    ratios = []
    for i in range(num_iterations):
        """
        #Z1 = np.dot(parameters["W1"], X.T) + parameters["b1"]
        #A1 = actFun(Z1)
        #Z2 = np.dot(parameters["W2"], A1) + parameters["b2"]
        #A2 = actFun(Z2)
        #Z3 = np.dot(parameters["W3"], A2) + parameters["b3"]
        """
        Z1, A1, Z2, A2, Z3 = forward_propagation(X, parameters, actFun)

        loss = np.mean((Z3 - Y.T) ** 2)
        loss1 = np.mean(np.abs(Z3 - Y.T))
        losses.append(loss)
        """
        dZ3 = 2 * (Z3 - Y.T)
        dW3 = np.dot(dZ3, A2.T) / input_size
        db3 = np.mean(dZ3, axis=1, keepdims=True)
        dA2 = np.dot(parameters["W3"].T, dZ3)
        dZ2 = actFunDer(dA2, Z2)
        dW2 = np.dot(dZ2, A1.T) / input_size
        db2 = np.mean(dZ2, axis=1, keepdims=True)
        dA1 = np.dot(parameters["W2"].T, dZ2)
        dZ1 = actFunDer(dA1, Z1)
        dW1 = np.dot(dZ1, X) / input_size
        db1 = np.mean(dZ1, axis=1, keepdims=True)

        grads = {"dW1": dW1, "db1": db1, "dW2": dW2, "db2": db2, "dW3": dW3, "db3": db3}
        """
        grads = backward_propagation(X, Y, Z1, A1, Z2, A2, Z3, parameters, actFunDer)
        t += 1
        parameters, v, s = update_parameters_with_adam(
            parameters, grads, v, s, t, learning_rate
        )

        if i % 10 == 0:
            loss = np.mean((Z3 - Y.T) ** 2)
            loss1 = np.mean(np.abs(Z3 - Y.T))
            # print(np.mean(np.abs(Z3[0] - Y.T[0])), end=" ")
            # print(np.mean(np.abs(Z3[1] - Y.T[1])), end=" ")
            # print(np.mean(np.abs(Z3[2] - Y.T[2])), end=" ")
            # print(np.mean(np.abs(Z3[3] - Y.T[3])))
            ratio = loss1 / np.sqrt(loss)
            ratios.append(ratio)
            print(f"MAE to RMSE ratio: {ratio}")

        if(i % 50 == 0):
            sum = 0
            for i in range(len(ratios)):
                sum += ratios[i]
            print(sum/len(ratios))


    return parameters


def main_training():
    data = get_data()
    data1 = data.to_numpy()
    np.random.shuffle(data1)

    X = data1[:training_size, :input_size]
    y = data1[:training_size, input_size:]

    train_features = X
    train_labels = y
    parameters = train_model(
        train_features,
        train_labels,
        hidden_size1=64,
        hidden_size2=64,
        num_iterations=1000,
        learning_rate=0.05,
        actFun=sigmoid,
        actFunDer=sigmoid_derivative,
    )

    with open("parameters3.pkl", "wb") as f:
        pickle.dump(parameters, f)

    test_features = data1[training_size:, :input_size]
    test_results = data1[training_size:, input_size:]

    _, _, _, _, predictions = forward_propagation(test_features, parameters, sigmoid)
    loss = np.mean((predictions - test_results.T) ** 2)
    print("Testing loss: " + str(loss))
    print(predictions)


def check_result():
    data = get_data()
    result = np.zeros((11, 4))
    realResults = np.zeros((11, 4))
    # testing_data = np.array([[0.27,0.05,0.6,0.05,0.27,197,3,0,0]])
    testing_data = np.array([[0.71, 0.49, 0.27, 0.27, 0.6, 197, 3, 0, 0]])
    rowInd = data[
        (data["InfDeadRate"] == testing_data[0][0])
        & (data["InfRate"] == testing_data[0][1])
        & (data["ReInfRate"] == testing_data[0][2])
        & (data["RecovRate"] == testing_data[0][3])
        & (data["SympRate"] == testing_data[0][4])
        & (data["Dead"] == testing_data[0][5])
        & (data["Infected"] == testing_data[0][6])
        & (data["Notinfecte"] == testing_data[0][7])
        & (data["Recovered"] == testing_data[0][8])
    ].index.tolist()
    result[0] = np.array([197, 3, 0, 0])
    realResults[0] = np.array([197, 3, 0, 0])
    with open("parameters4.pkl", "rb") as f:
        parameters = pickle.load(f)
    for i in range(1, 11):
        predictions = forward_propagation(testing_data.T, parameters)
        predictions = np.rint(predictions)
        sum1 = np.sum(predictions)
        for j in range(4):
            predictions[j] = (predictions[j] / sum1) * 200
            testing_data[0, 5 + j] = predictions[j]
        predictions = np.rint(predictions)
        result[i] = predictions.flatten()
        realResults[i] = data.iloc[rowInd[0] + i - 1][-4:]

    result = np.round(result)
    result[result < 0] = 0
    print(result)

    notInfected = result[:, 0]
    infected = result[:, 1] + notInfected
    recovered = result[:, 2] + infected
    dead = result[:, 3] + recovered
    plotRes(notInfected, infected, recovered, dead, "Przewidywany stan epidemii")
    notInfected = realResults[:, 0]
    infected = realResults[:, 1] + notInfected
    recovered = realResults[:, 2] + infected
    dead = realResults[:, 3] + recovered
    plotRes(notInfected, infected, recovered, dead, "Rzeczywisty stan epidemii")


def plotRes(notInfected, infected, recovered, dead, title):
    plt.figure()
    plt.plot(notInfected, color="yellow", label="Zdrowi")
    plt.plot(infected, color="red", label="Zarażeni")
    plt.plot(recovered, color="green", label="Wyzdrowiali")
    plt.plot(dead, color="grey", label="Martwi")
    plt.fill_between(np.arange(len(dead)), dead, color="#808080", alpha=1)  # szary
    plt.fill_between(
        np.arange(len(recovered)), recovered, color="#66CD00", alpha=1
    )  # zielony
    plt.fill_between(
        np.arange(len(infected)), infected, color="#FF4500", alpha=1
    )  # czerwony
    plt.fill_between(
        np.arange(len(notInfected)), notInfected, color="#FFD700", alpha=1
    )  # zolty
    plt.title(title)
    plt.legend()
    plt.xlabel("Rok")
    plt.ylabel("Ilość")


losses = []

# check_result()
plt.show()
main_training()
plt.plot(losses)
plt.yscale("log")
plt.title("Wartość fukcji błędu - własna implementacja sieci")
plt.ylabel("Wartość błędu")
plt.xlabel("Numer iteracji")
time = datetime.now()
plt.savefig("plots/" + datetime.now().strftime("%Y%m%d%H%M%S"))
plt.show()

os.system("pause")