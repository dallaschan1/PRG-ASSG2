using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment2
{
    internal class PointCard
    {
        public int points { get; set; }

        public int punchCard { get; set; }

        public string tier { get; set; }

        public PointCard()
        {
            points = 0;
            punchCard = 0;

        }

        public PointCard(int points, int punchCard)
        {

            this.points = points;
            this.punchCard = punchCard;
            tier = "Ordinary";
        }

        public void AddPoints(int money)
        {

            points += (int)Math.Floor(money * 0.72);
            if (points >= 100 && tier != "Gold")
            {
                tier = "Gold";
            }
            else if (points >= 50 && tier == "Ordinary")
            {
                tier = "Silver";
            }

        }

        public void RedeemPoints(int amount)
        {
            points -= amount;
        }

        public void Punch()
        {
            if (punchCard == 10)
            {
                punchCard = 0;
            }
            else { punchCard += 1; }
        }
        public override string ToString()
        {
            return $"points: {points}, punchCard: {punchCard}, tier: {tier}";
        }

    }
}
