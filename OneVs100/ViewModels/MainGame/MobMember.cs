using System;
using Avalonia.Controls;
using OneVs100.CustomControls;
using OneVs100.Helpers;

namespace OneVs100.ViewModels;

public class MobMember
{
    public int Number { get; init; }
    private readonly float intelligence;
    private char answer = ' ';
    public bool IsKnockedOut = false;
    private RandomGaussian RNG;
    

    public MobMember(int number, RandomGaussian RNG)
    {
        this.RNG = RNG;
        this.Number = number;
        //this.intelligence = RNG.Next(0, 36)*RNG.NextSingle();
        this.intelligence = Convert.ToSingle(RNG.BoxMuller(0, 20, spread: 3));
    }

    public void SelectAnswer(char correctAnswer, float difficulty, int questionNumber)
    {
        this.answer = questionNumber * difficulty < intelligence
            ? correctAnswer
            : RNG.GetItems(['A', 'B', 'C'], 1)[0];
    }

    public bool IsAnswerCorrect(char correctAnswer) => (correctAnswer == answer);


    //Knock-out calculation - Difficulty*QuestionNr > Intelligence
}