using System;
using System.Collections.Generic;
using UnityEngine;

/* ��� ������: ������� ��� �����, ���� ����� �������� �����������
 *  ����������: �������� �� private/protected/��������� ���� ������
 * */
[AttributeUsage(AttributeTargets.Field)]
public class InjectAttribute : Attribute { }


public class SceneRootBinder : MonoBehaviour
{
    // ������� ������������������ ������������
    private readonly Dictionary<Type, object> _container = new Dictionary<Type, object>();

    // ����������� ���������� � container
    public void Register<T>(T instance)
    {
        if (instance == null)
        {
            Debug.LogError($"Trying to register NULL instance of {typeof(T).Name}!");
        }
        var type = typeof(T);
        if (_container.ContainsKey(type))
        {
            Debug.LogWarning($"Component of type {type.Name} already registered, overriding.");
        }
        _container[type] = instance;
        //Debug.Log($"Registered {type.Name} in DI container");

    }

    // ��������� ���������� �� container
    public T Resolve<T>()
    {
        var type = typeof(T);
        if (_container.TryGetValue(type, out var instance))
        {
            return (T)instance;
        }
        Debug.LogError($"Failed to resolve dependency of type {type.Name}");
        return default;
    }

    //��� ������ � �������� �������� ����� ������������ zenject � ������
    public void InjectDependencies(object target)
    {

        var type = target.GetType();
        var fields = type.GetFields(System.Reflection.BindingFlags.NonPublic |
                                   System.Reflection.BindingFlags.Public |
                                   System.Reflection.BindingFlags.Instance);


        foreach (var field in fields)
        {
            var injectAttribute = Attribute.GetCustomAttribute(field, typeof(InjectAttribute)) as InjectAttribute;
            if (injectAttribute != null)
            {
                var fieldType = field.FieldType;
                //                      ���� ���� ����� ������� [Inject] �����������
                if (_container.TryGetValue(fieldType, out var dependency))
                {
                    //                      ����������� �� ����������
                    field.SetValue(target, dependency);
                    //Debug.Log($"Injected {fieldType.Name} into {type.Name}.{field.Name}");
                }
                else
                {
                    Debug.LogError($"Failed to inject dependency of type {fieldType.Name} into {type.Name}.{field.Name}");
                }
            }
        }
    }

}

