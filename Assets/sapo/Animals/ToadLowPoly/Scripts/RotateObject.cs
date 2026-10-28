using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 50.0f; // Заданная скорость вращения объекта
    public float rotationSpeedChangeAmount = 10.0f; // Изменение скорости вращения

    private bool isRotating = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0)) // Проверяем нажатие клавиши "0"
        {
            ToggleRotation(); // Переключаем состояние вращения
        }

        if (Input.GetKeyDown(KeyCode.Minus)) // Проверяем нажатие клавиши "-"
        {
            AdjustRotationSpeed(-rotationSpeedChangeAmount); // Уменьшаем скорость вращения
        }

        if (Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus)) // Проверяем нажатие клавиши "+" или "="
        {
            AdjustRotationSpeed(rotationSpeedChangeAmount); // Увеличиваем скорость вращения
        }

        if (isRotating)
        {
            RotateY();
        }
    }

    void ToggleRotation()
    {
        isRotating = !isRotating; // Инвертируем состояние вращения
    }

    void AdjustRotationSpeed(float amount)
    {
        rotationSpeed += amount; // Изменяем скорость вращения на заданное количество
        rotationSpeed = Mathf.Max(rotationSpeed, 0.0f); // Убеждаемся, что скорость не станет отрицательной
    }

    void RotateY()
    {
        float rotationAmount = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotationAmount);
    }
}