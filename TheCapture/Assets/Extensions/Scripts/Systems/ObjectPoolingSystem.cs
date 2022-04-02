using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public  class ObjectPoolingSystem : MonoBehaviour
{
    public GameObject bulletPrefab;


    private List<GameObject> pooledObjects = new List<GameObject>();
    
    
    public void InstantiatePool( int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject _bullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);
            _bullet.SetActive(false);
            pooledObjects.Add(_bullet);
        }
    }
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

}
