using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitScript : NetworkBehaviour
{
    public int unitType;
    [SerializeField] private int movementRadius;
    private int movementLeft;
    [SerializeField] private int attackRadius;
    [SyncVar (hook = nameof(Damaged))] public float currentHealth;
    private float maxHealth;
    public float armor;
    [SerializeField] private float damage;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float offsetPosicionX;
    [SerializeField] private float offsetPosicionY;

    private Animator animator;

    private List<Vector2> movementTiles;
    private List<Vector2> attackTiles;
    private bool canMove = true;
    private bool isSelected = false;
    private bool isMoving = false;
    private bool flipped = false;
    private bool isAttacking = false;
    private string xDirection = "";
    private string yDirection = "";
    private UnitScript targetUnit;

    public event Action<float> OnHealthChanged = delegate { };

    void Start()
    {
        // agrega automaticamente el script a la lista de unidades
        BattleManager.instance.UnitInstantiated(this);

        animator = GetComponent<Animator>();

        movementTiles = new List<Vector2>();
        attackTiles = new List<Vector2>();

        movementLeft = movementRadius;
    }

    private void OnEnable() => maxHealth = currentHealth;

    public bool Selected()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            if (hit.collider != null && hit.collider.transform == transform)
                isSelected = true;
        }

        return isSelected;
    }

    public void flipUnit()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        flipped = !flipped;
    }

    public void Move(Vector3 position)
    {
        if (xDirection == "derecha" && flipped || xDirection == "izquierda" && !flipped)
            flipUnit();

        if (!isMoving) { return; }

        float currentXPosition = transform.position.x - offsetPosicionX;
        float currentYPosition = transform.position.y - offsetPosicionY;
        Vector3 currentPosition = new Vector3(currentXPosition, currentYPosition);

        animator.SetBool("moviendo", true);

        if (xDirection == "derecha")
        {
            // comprueba que el eje no haya llegado a su destino
            if (currentPosition.x < position.x)
                // en ese caso, se ejecuta el movimiento
                transform.Translate(movementSpeed * Time.deltaTime, 0, 0);
            else if (currentPosition.x >= position.x)
                xDirection = "";
        }
        else if (xDirection == "izquierda")
        {
            if (currentPosition.x > position.x)
                transform.Translate(-movementSpeed * Time.deltaTime, 0, 0);
            else if (currentPosition.x <= position.x)
                xDirection = "";
        }

        // if (direccionX == "") // para que solo mueva en eje 'x' antes que 'y'
        if (yDirection == "arriba")
        {
            if (currentPosition.y < position.y)
                transform.Translate(0, movementSpeed * Time.deltaTime, 0);
            else if (currentPosition.y >= position.y)
                yDirection = "";
        }
        else if (yDirection == "abajo")
        {
            if (currentPosition.y > position.y)
                transform.Translate(0, -movementSpeed * Time.deltaTime, 0);
            else if (currentPosition.y <= position.y)
                yDirection = "";
        }

        // compueba que ambos ejes hayan llegado a su destino
        if (xDirection == "" && yDirection == "")
        {
            animator.SetBool("moviendo", false);
            isMoving = false;

            // posiciona al personaje en su lugar, ya que es posible que se pase de su destino por algunos pixeles
            transform.position = new Vector3(position.x + offsetPosicionX, position.y + offsetPosicionY, transform.position.z);

            // si no tiene movimientos disponibles...
            if (movementLeft == 0)
                canMove = false;
        }
    }

    public void Attack(UnitScript target)
    {
        if (!isAttacking && !isMoving) { return; }

        targetUnit = target;
        isAttacking = false;
        isSelected = false;

        animator.SetTrigger("atacando");
    }

    public void Hit()
    {
        // comprueba que la conexion tenga autoridad sobre la unidad
        if (!hasAuthority) { return; };

        ConnectionManager.instance.CmdAttackUnit(targetUnit.gameObject, damage);
    }

    public void Damaged(float oldValue, float newValue)
    {
        currentHealth = newValue;

        // calcula el porcentaje de la vida que tiene
        float currentHealthPct = currentHealth / maxHealth;

        // actualiza la barra de vida
        OnHealthChanged(currentHealthPct);

        if (currentHealth <= 0)
            animator.SetTrigger("muriendo");
        else
            animator.SetTrigger("golpeado");
    }

    public void SetMovementDirection(Vector3 position)
    {
        if (isSelected)
        {
            float currentXPosition = transform.position.x - offsetPosicionX;
            float currentYPosition = transform.position.y - offsetPosicionY;
            Vector3 currentPosition = new Vector3(currentXPosition, currentYPosition);

            if (currentPosition != position)
            {
                float tilesToMoveInPixels = 0f;

                if (currentPosition.x < position.x)
                {
                    xDirection = "derecha";
                    tilesToMoveInPixels = -currentPosition.x +position.x;
                }  
                else if (currentPosition.x > position.x)
                {
                    xDirection = "izquierda";
                    tilesToMoveInPixels = -position.x + currentPosition.x;
                }

                if (currentPosition.y < position.y)
                {
                    yDirection = "arriba";
                    tilesToMoveInPixels = -currentPosition.y + position.y;
                }
                else if (currentPosition.y > position.y)
                {
                    yDirection = "abajo";
                    tilesToMoveInPixels = -position.y + currentPosition.y;
                }

                // calcula y resta la cantidad de tiles que se movera la unidad
                movementLeft -= (int)tilesToMoveInPixels / 127;

                isMoving = true;
                isSelected = false;
            }
            else
                isMoving = false;
        }
    }

    public void SetTilesRadius(Vector3Int gridUnitPos)
    {
        // determina la posicion del primer tile de iteracion
        int initialXPos = gridUnitPos.x + 1;
        int initialYPos = gridUnitPos.y - movementLeft;

        // determina cuantas filas tiene el radio
        int rows = initialYPos + (movementLeft * 2) + 1;
        int tilesPerRow = initialXPos - 1;

        for (int yPos = initialYPos; yPos < rows; yPos++)
        {
            // dependiendo de posY, setea cuantos tiles debe tener la fila
            if (yPos <= gridUnitPos.y)
            {
                tilesPerRow++;
                initialXPos--;
            }
            else
            {
                tilesPerRow--;
                initialXPos++;
            }

            for (int xPos = initialXPos; xPos < tilesPerRow; xPos++)
            {
                // evita guardar la posicion del jugador
                if (xPos == gridUnitPos.x && yPos == gridUnitPos.y)
                    continue;

                // comprueba que el tile sea valido para mover
                if (BattleManager.instance.CanMoveToPosition(new Vector3Int(xPos, yPos, 0)))
                    movementTiles.Add(new Vector2(xPos, yPos));
            }
        }

        // remueve los tiles de movimiento que son inaccesibles debido a unidades enemigas
        RemoveInaccesibleTiles(gridUnitPos);
    }

    private void RemoveInaccesibleTiles(Vector3Int gridUnitPos)
    {
        List<Vector2> validTilesList = new List<Vector2>();
        validTilesList.AddRange(movementTiles);

        foreach (Vector2 tilePos in movementTiles)
        {
            // comprueba si la posicion colisiona con alguna unidad
            if (!BattleManager.instance.IsUnitInPosition(new Vector3Int((int)tilePos.x, (int)tilePos.y, 0))) { continue; }

            // para evitar errores, por si el tile ya se habia borrado antes
            if (!validTilesList.Contains(tilePos)) { continue; }

            if (tilePos.x > gridUnitPos.x)
                for (float posX = tilePos.x; posX <= gridUnitPos.x + movementRadius; posX++)
                    validTilesList.Remove(new Vector2(posX, tilePos.y));

            if (tilePos.x < gridUnitPos.x)
                for (float posX = tilePos.x; posX >= gridUnitPos.x - movementRadius; posX--)
                    validTilesList.Remove(new Vector2(posX, tilePos.y));

            if (tilePos.y > gridUnitPos.y)
                // borra todos los tiles desde el invalido en adelante
                for (float posY = tilePos.y; posY <= gridUnitPos.y + movementRadius; posY++)
                    validTilesList.Remove(new Vector2(tilePos.x, posY));

            if (tilePos.y < gridUnitPos.y)
                for (float posY = tilePos.y; posY >= gridUnitPos.y - movementRadius; posY--)
                    validTilesList.Remove(new Vector2(tilePos.x, posY));
        }

        movementTiles = validTilesList;

        // agrega los tiles de ataque a los tiles de movimiento actuales
        SetAttackTiles(gridUnitPos);
    }

    private void SetAttackTiles(Vector3Int gridUnitPos)
    {
        // comprueba si se le puede añadir un tile de ataque al lado de cada tile de movimiento
        foreach (Vector2 tilePos in movementTiles)
        {
            CheckTileIsValidForAttack(new Vector2(tilePos.x + 1, tilePos.y), gridUnitPos);
            CheckTileIsValidForAttack(new Vector2(tilePos.x - 1, tilePos.y), gridUnitPos);
            CheckTileIsValidForAttack(new Vector2(tilePos.x, tilePos.y + 1), gridUnitPos);
            CheckTileIsValidForAttack(new Vector2(tilePos.x, tilePos.y - 1), gridUnitPos);
        }

        // hace lo mismo para las posiciones correlativas a la unidad
        CheckTileIsValidForAttack(new Vector2(gridUnitPos.x + 1, gridUnitPos.y), gridUnitPos);
        CheckTileIsValidForAttack(new Vector2(gridUnitPos.x - 1, gridUnitPos.y), gridUnitPos);
        CheckTileIsValidForAttack(new Vector2(gridUnitPos.x, gridUnitPos.y + 1), gridUnitPos);
        CheckTileIsValidForAttack(new Vector2(gridUnitPos.x, gridUnitPos.y - 1), gridUnitPos);
    }

    private void CheckTileIsValidForAttack(Vector2 tileToCheck, Vector3Int gridUnitPos)
    {
        Vector3Int tileToCheckInTilemap = new Vector3Int((int)tileToCheck.x, (int)tileToCheck.y, 0);
        // comprueba que no haya un tile de movimiento, ataque o o la unidad en la posicion
        if (tileToCheckInTilemap != gridUnitPos)
            if (!movementTiles.Contains(tileToCheck) && !attackTiles.Contains(tileToCheck))
                if (BattleManager.instance.CanMoveToPosition(tileToCheckInTilemap))
                    attackTiles.Add(tileToCheck);
    }

    public bool ClickOnMovementTile(int x, int y)
    {
        foreach (Vector2 posTile in movementTiles)
            if (posTile.x == x && posTile.y == y)
                return true;

        return false;
    }

    public bool ClickOnAttackTile(int x, int y)
    {
        foreach (Vector2 tilePosition in attackTiles)
            if (tilePosition.x == x && tilePosition.y == y)
                return true;

        return false;
    }

    public List<Vector2> GetMovementTiles() => movementTiles;

    public List<Vector2> GetAttackTiles() => attackTiles;

    public void ToggleSelected(bool state) => isSelected = state;

    public void ToggleAttack(bool state) => isAttacking = state;

    public bool IsSelected() => isSelected;

    public bool CanMove() => canMove;

    public void ResetUnitsInArmy()
    {
        movementLeft = movementRadius;
        canMove = true;
    }

    public bool IsMoving() => isMoving;

    public bool IsDead() {
        if (currentHealth < 0)
            return true;
        else
            return false;
    }

    public void DestroyUnit() => Destroy(gameObject);

    public Vector3 GetPosition()  => transform.position - new Vector3(offsetPosicionX, offsetPosicionY, 0f);
    
    public void Deploy(Vector3 position) => transform.position = 
        new Vector3(position.x + offsetPosicionX, position.y + offsetPosicionY, transform.position.z);
}
