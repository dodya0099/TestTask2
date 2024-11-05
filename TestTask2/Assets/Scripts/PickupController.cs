using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    public float pickupRange = 3f;          // Дистанция, на которой можно поднимать объект
    public float throwForce = 10f;          // Сила, с которой объект будет брошен
    public LayerMask pickupLayer;           // Слой подбираемых объектов
    private Transform pickedObject = null;  // Текущий поднимаемый объект
    private Outline currentOutline = null;  // Текущий объект с активным Outline

    void Update()
    {
        // Проверяем наличие объекта для поднятия при наведении
        CheckForObjectToHighlight();

        // Проверяем нажатие и отпускание мыши
        if (Input.GetMouseButtonDown(0) && pickedObject == null)
        {
            TryPickUp();
        }
        else if (Input.GetMouseButtonUp(0) && pickedObject != null)
        {
            ThrowObject();
        }
    }

    void CheckForObjectToHighlight()
{
    // Создаем луч из центра экрана
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    // Проверяем, попадает ли луч в объект на нужном расстоянии и слое
    if (Physics.Raycast(ray, out hit, pickupRange, pickupLayer))
    {
        Transform target = hit.transform;

        // Если объект изменился, сбросим подсветку у предыдущего
        if (currentOutline != null && currentOutline.transform != target)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }

        // Ищем компонент Outline у нового объекта
        Outline outline = target.GetComponent<Outline>();
        if (outline != null)
        {
            outline.enabled = true;
            currentOutline = outline;
        }
    }
    else if (currentOutline != null)
    {
        // Отключаем подсветку, если луч не попал в подбираемый объект
        currentOutline.enabled = false;
        currentOutline = null;
    }
}

    void TryPickUp()
    {
        if (currentOutline != null) // Проверяем, что объект подсвечен
        {
            pickedObject = currentOutline.transform;
            pickedObject.GetComponent<Outline>().enabled = false;
            Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // Отключаем физику, чтобы объект не падал
            }

            pickedObject.SetParent(Camera.main.transform); // Делаем объект дочерним камере
            pickedObject.localPosition = new Vector3(0, 0, pickupRange - 1); // Позиционируем перед камерой
            currentOutline.enabled = false; // Отключаем подсветку у поднятого объекта
            currentOutline = null; // Очищаем ссылку на Outline
        }
    }

    void ThrowObject()
    {
        if (pickedObject != null)
        {
            Rigidbody rb = pickedObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false; // Включаем физику
                rb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse); // Придаем силу броска
            }

            pickedObject.SetParent(null); // Убираем объект из дочерних камеры
            pickedObject = null; // Очищаем ссылку на объект
        }
    }
}
