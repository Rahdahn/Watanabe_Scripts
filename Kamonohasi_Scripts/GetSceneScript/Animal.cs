using UnityEngine;

public enum AnimalType
{
    D1, D2, D3, D4, D5
}

public class Animal : MonoBehaviour
{
    public AnimalType type;
    private AnimalMovement animalMovement; // AnimalMovement コンポーネントへの参照

    private void Start()
    {
        animalMovement = GetComponent<AnimalMovement>();
    }

    public void StopMoving()
    {
        if (animalMovement != null)
        {
            animalMovement.StopMoving(); // AnimalMovement の StopMoving メソッドを呼び出す
        }
    }

    public void StartMoving()
    {
        if (animalMovement != null)
        {
            animalMovement.StartMoving(); // AnimalMovement の StartMoving メソッドを呼び出す
        }
    }
}
