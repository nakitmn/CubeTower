using UnityEngine;

public class SeparateTower : MonoBehaviour
{

    public float ExposionPower = 70f;
    public float ExposionRadius = 5f;
    private bool IsCollised = false;

    public GameObject RestartButton;
    public GameObject ExposionEffect;
    public float ShakeTime=1f, ShakePower=0.05f, ShakeDisapearDelay=1.5f;
    private Vector3 PreviousCameraPosition;

    private void OnCollisionEnter(Collision collision)
    {
        // Если коснулся елемент с тегом "Tower"
        if (collision.gameObject.tag == "Tower" && !IsCollised)
        {
            // Цикл по всем кубикам башенки
            for(int i = collision.gameObject.transform.childCount -1; i >= 0; i--)
            {
                Transform child = collision.gameObject.transform.GetChild(i);

                // Добавляем к кубику башни компонент "Rigidbody"
                child.gameObject.AddComponent<Rigidbody>();
                // Задаем в "Rigidbody" значения для взрыва. AddExplosionForce - добавляет взырвную силу.
                // 1 параметр - сила взрыва
                // 2 параметр - направление силы (Vector3.up - Y)
                // 3 параметр - радиус действия
                child.gameObject.GetComponent<Rigidbody>().AddExplosionForce(ExposionPower, Vector3.up, ExposionRadius);

                // Отсоединяем кубик от других кубиков
                child.SetParent(null);
            }
            RestartButton.SetActive(true);
            Destroy(collision.gameObject); // Удаляем объект, который столкнулся с Ground (Tower)
            Camera.main.transform.localPosition -= new Vector3(0,0,Camera.main.transform.localPosition.y ); // Если нашал камера вложена в какой-либо объект,
            // ТО необходимо использовать localPosition вместо Position!!!
            PlaySoundEffect();
            IsCollised = true; // Помечаем, что наша башня уже упала, дабы избежать всяких казусов
            PreviousCameraPosition = Camera.main.transform.localPosition;


            GameObject ExpoisonEffectObject = Instantiate(ExposionEffect,
                        new Vector3(collision.contacts[0].point.x, collision.contacts[0].point.y, collision.contacts[0].point.z), // Точка прикосновения Ground и Tower
                        Quaternion.identity) as GameObject; // Создаем эффект при столкновении Tower c Ground
            Destroy(ExpoisonEffectObject, 1.5f);  // Удаляем этот эффект через 2 секунды, чтобы освоблодить память
        }
    }


    private void ShakeCamera()
    {
        Camera.main.transform.localPosition = PreviousCameraPosition + Random.insideUnitSphere * ShakePower;
        ShakeTime -= ShakeDisapearDelay * Time.deltaTime;
    }

    private void Update()
    {
        if (IsCollised)
        {
            if (ShakeTime > 0) ShakeCamera();
            else Camera.main.transform.localPosition = PreviousCameraPosition;
        }
    }

    private void PlaySoundEffect()
    {
        if (PlayerPrefs.GetString("music") != "No") GetComponent<AudioSource>().Play();

    }
}
