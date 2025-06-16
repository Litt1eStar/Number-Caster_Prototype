using UnityEngine;

[CreateAssetMenu(fileName = "ClassSO", menuName = "Scriptable Objects/ClassSO")]
public class ClassSO : ScriptableObject
{
    public string ClassName;
    public string ClassDescription;
    public SkillSO Skill;
}
