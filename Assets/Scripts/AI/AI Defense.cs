using UnityEngine;

namespace ITKombat
{
    public class AI_Defense : MonoBehaviour
    {

        public bool isBlocking = false;
        public float criticalProximity = 2.5f;
        public float blockChance = 0.2f;
        public float blockDuration = 2f;
        
        private AI_Movement aiMovement;
        private AI_Attack aiAttack;
        private Animator animation;
        void Start()
        {
            aiAttack = GetComponent<AI_Attack>();
            aiMovement = GetComponent<AI_Movement>();
            animation = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if(isBlocking == true){
                animation.SetTrigger("Block");
                aiMovement.canMove = false;
                aiAttack.canAttack = false;
                
                // Debug.Log("asjjjjjjjjj");
            }
            else
            {
                aiAttack.canAttack = true;
                aiMovement.canMove = true;
                // Debug.Log("sallllll");
            }
        }
    }
}
