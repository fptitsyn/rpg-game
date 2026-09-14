namespace Actors.Enemies
{
    public enum EnemyIntent { Idle, Approach, Retreat, Attack }

    public static class EnemyDecision
    {
        public static EnemyIntent Evaluate(float distance, bool targetAlive, bool ranged, bool clearShot,
            float detection = 10, float meleeRange = 2, float minimumRange = 5, float maximumRange = 10)
        {
            if (!targetAlive || distance > detection) return EnemyIntent.Idle;
            if (!ranged)
                return distance <= meleeRange && clearShot ? EnemyIntent.Attack : EnemyIntent.Approach;
            if (distance < minimumRange) return EnemyIntent.Retreat;
            if (distance > maximumRange || !clearShot) return EnemyIntent.Approach;
            return EnemyIntent.Attack;
        }
    }
}
