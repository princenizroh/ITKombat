using UnityEngine;

namespace ITKombat
{
    public class AI_Defense : MonoBehaviour
    {
        public bool isBlocking = false;
        public float criticalProximity = 3f;
        public float blockChance = 0.2f;
        public float blockDuration = 0.7f;
        [Header("Others")]
        private AI_Movement aiMovement;
        private AI_Attack aiAttack;
        private Animator anim;
        void Start()
        {
            aiAttack = GetComponent<AI_Attack>();
            aiMovement = GetComponent<AI_Movement>();
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            if(isBlocking == true){
                anim.SetTrigger("Block");
                aiMovement.canMove = false;
                aiAttack.canAttack = false;
            }
            else
            {
                aiAttack.canAttack = true;
                aiMovement.canMove = true;
            }
        }
    }
}
