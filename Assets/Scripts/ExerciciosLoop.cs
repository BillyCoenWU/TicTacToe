using UnityEngine;

public class ExerciciosLoop : MonoBehaviour
{
	[SerializeField]
	private int m_exerciciosIndex = 0;

	private void Start ()
	{
		switch(m_exerciciosIndex)
		{
			case 1:
				ExercicioOne();
				break;

			case 2:
				ExercicioTwo();
				break;

			case 3:
				ExercicioThree();
				break;

			case 4:
				ExercicioFour();
				break;

			default:
				ExercicioOne();
				ExercicioTwo();
				ExercicioThree();
				break;
		}
	}

	private void ExercicioOne ()
	{
		int soma = 0;

		for(int i = 1; i <= 1000; i++)
		{
			soma += i;
		}

		Debug.Log(soma);
	}

	private void ExercicioTwo ()
	{
		//int soma = 0;

		for(int i = 1; i <= 100; i++)
		{
			if(i % 3 == 0)
			{
				Debug.Log(i);
				//soma += i;
			}
		}



		//Debug.Log(soma);
	}

	private void ExercicioThree ()
	{
		int soma = 0;

		for(int i = 1; i <= 100; i++)
		{
			if(i % 3 != 0)
			{
				soma += i;
			}
		}

		Debug.Log(soma);
	}


	private void ExercicioFour ()
	{
		Debug.Log(Fatorial(6));
	}

	private int Fatorial (int i)
	{
		int n = i;
		
		while(i > 2)
		{
			n *= i-1;
			i--;
		}

		return n;
	}
}
