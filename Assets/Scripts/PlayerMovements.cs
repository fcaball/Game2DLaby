using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    [SerializeField] private float PlayerSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.W)){
            transform.position+=Vector3.up*Time.deltaTime*PlayerSpeed;
        }
        if(Input.GetKey(KeyCode.S)){
            transform.position-=Vector3.up*Time.deltaTime*PlayerSpeed;
        }
        if(Input.GetKey(KeyCode.A)){
            transform.position-=Vector3.right*Time.deltaTime*PlayerSpeed;
        }
        if(Input.GetKey(KeyCode.D)){
            transform.position+=Vector3.right*Time.deltaTime*PlayerSpeed;
        }
    }
}
