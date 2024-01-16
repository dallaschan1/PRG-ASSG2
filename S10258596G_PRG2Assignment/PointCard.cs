using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    internal class PointCard
    {
        public int Points { get; set; }

        public int PunchCard { get; set; }

        public string Tier { get; set; }

        public PointCard()
        {
            Points = 0;
            PunchCard = 0;
            Tier = "Ordinary";
        }

        public PointCard(int points, int punchCard)
        {

            Points = points;
            PunchCard = punchCard;
            if (Points >= 100)
            {
                Tier = "Gold";
            }
            else if (Points >= 50)
            {
                Tier = "Silver";
            }
            else
            {
                Tier = "Ordinary";
            }
        }

        public void AddPoints(int money)
        {
            Points += (int)Math.Floor(money * 0.72);
            if (Points >= 100 && Tier != "Gold")
            {
                Tier = "Gold";
            }
            else if (Points >= 50 && Tier == "Ordinary")
            {
                Tier = "Silver";
            }
        }

        public void RedeemPoints(int amount)
        {
            Points -= amount;
        }

        public void Punch()
        {
            if (PunchCard == 10)
            {
                PunchCard = 0;
            }
            else { PunchCard += 1; }
        }
        public override string ToString()
        {
            return $"points: {Points}, punchCard: {PunchCard}, tier: {Tier}";
        }

    }
}
