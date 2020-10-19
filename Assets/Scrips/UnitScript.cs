using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class UnitScript : NetworkBehaviour
{
    public int unitType;
    [SerializeField] private int movementRadius;
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

    public void Move(Vector3 position)
    {
        if (xDirection == "derecha" && flipped || xDirection == "izquierda" && !flipped)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            flipped = !flipped;
        }

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
                if (currentPosition.x < position.x)
                    xDirection = "derecha";
                else if (currentPosition.x > position.x)
                    xDirection = "izquierda";

                if (currentPosition.y < position.y)
                    yDirection = "arriba";
                else if (currentPosition.y > position.y)
                    yDirection = "abajo";

                isMoving = true;
                isSelected = false;
            }
            else
                isMoving = false;
        }
    }

    public void SetTileRadius(int unitXPosition, int unitYPosition)
    {
        // determina la posicion del primer tile de iteracion
        int initialXPos = unitXPosition + 1;
        int initialYPos = unitYPosition - movementRadius;

        // determina cuantas filas tiene el radio
        int rows = (movementRadius * 2) + 1 + initialYPos;
        int tlesPerRow = initialXPos - 1;

        for (int yPos = initialYPos; yPos < rows; yPos++)
        {
            // dependiendo de posY, setea cuantos tiles debe tener la fila
            if (yPos <= unitYPosition)
            {
                tlesPerRow++;
                initialXPos--;
            }
            else
            {
                tlesPerRow--;
                initialXPos++;
            }

            for (int xPos = initialXPos; xPos < tlesPerRow; xPos++)
            {
                // evita guardar la posicion del jugador
                if (xPos == unitXPosition && yPos == unitYPosition)
                    continue;

                movementTiles.Add(new Vector2(xPos, yPos));
            }
        }
    }

    public bool ClickOnMovementTile(int x, int y)
    {
        foreach (Vector2 posTile in movementTiles)
        {
            if (posTile.x == x && posTile.y == y)
                return true;
        }

        return false;
    }

    public bool ClickOnAttackTile(int x, int y)
    {
        foreach (Vector2 tilePosition in attackTiles)
        {
            if (tilePosition.x == x && tilePosition.y == y)
                return true;
        }

        return false;
    }

    public List<Vector2> GetMovementTiles() => movementTiles;

    public List<Vector2> GetAttackTiles() => attackTiles;

    public void SetMovementTiles(List<Vector2> newMovementTiles) => movementTiles = newMovementTiles;

    public void SetAttackTiles(List<Vector2> newAttackTiles) => attackTiles = newAttackTiles;

    public int GetMovementRadius() => movementRadius;

    public void ToggleSelected(bool state) => isSelected = state;

    public void ToggleAttack(bool state) => isAttacking = state;

    public bool IsSelected() => isSelected;

    public bool IsMoving() => isMoving;

    public bool IsDead() {
        if (currentHealth < 0)
            return true;
        else
            return false;
    }

    public void DestroyUnit() => Destroy(gameObject);

    public Vector3 GetPosition()  => transform.position;

    public void Deploy(Vector3 position) => transform.position = 
        new Vector3(position.x + offsetPosicionX, position.y + offsetPosicionY, transform.position.z);
}
