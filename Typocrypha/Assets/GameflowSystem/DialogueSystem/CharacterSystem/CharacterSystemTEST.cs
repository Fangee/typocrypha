using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TypocryphaGameflow;

namespace TypocryphaGameflow
{
    // Testing script for Character System
    public class CharacterSystemTEST : MonoBehaviour
    {
        public CharacterManager characterManager;
        public CharacterData stick;

        IEnumerator Start()
        {
            AddCharacter add = ScriptableObject.CreateInstance(typeof(AddCharacter)) as AddCharacter;
            add.pos = Vector2.zero;
            add.startingPose = "Normal";
            add.startingExpression = "Happy";
            characterManager.characterControl("Stick", add);
            yield return new WaitForSeconds(1f);

            MoveCharacter move = ScriptableObject.CreateInstance(typeof(MoveCharacter)) as MoveCharacter;
            move.targetPos = new Vector2(2, 0);
            characterManager.characterControl("Sticky", move);
            yield return new WaitForSeconds(1f);

            RemoveCharacter remove = ScriptableObject.CreateInstance(typeof(RemoveCharacter)) as RemoveCharacter;
            characterManager.characterControl("Stick", remove);
        }
    }
}

