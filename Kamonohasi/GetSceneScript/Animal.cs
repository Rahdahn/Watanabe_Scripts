using UnityEngine;

public enum AnimalType
{
    D1, D2, D3, D4, D5
}

public class Animal : MonoBehaviour
{
    public AnimalType type;
    private AnimalMovement animalMovement; // AnimalMovement �R���|�[�l���g�ւ̎Q��

    private void Start()
    {
        animalMovement = GetComponent<AnimalMovement>();
    }

    public void StopMoving()
    {
        if (animalMovement != null)
        {
            animalMovement.StopMoving(); // AnimalMovement �� StopMoving ���\�b�h���Ăяo��
        }
    }

    public void StartMoving()
    {
        if (animalMovement != null)
        {
            animalMovement.StartMoving(); // AnimalMovement �� StartMoving ���\�b�h���Ăяo��
        }
    }
}
