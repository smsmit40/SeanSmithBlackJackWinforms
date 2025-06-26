using SeanSmithBlackJackWInforms;
using System.Net.Http.Json;

namespace SeanSmithBlackJackWInforms
{
    public partial class Form1 : Form
    {

        public static int BetAmount = 0;
        public static int HandTwoBetAmount = 0;
        public static int BankRoll = 10000;
        public static string deckId = "";
        public static int remainingCards = 99999;

        public static int PlayerHandOneValue = 0;
        public static int PlayerHandOneAces = 0;
        public static int PlayerHandTwoValue = 0;
        public static int PlayerHandTwoAces = 0;
        public static bool PlayerHandOneActive = false;
        public static bool PlayerHandTwoActive = false;
        public static int DealerHandValue = 0;
        public static int DealerHandAces = 0;
        public static int HiddenDealerHandValue = 0;
        public static string HiddenDealderHandUrl = "";


        public Form1()
        {
            InitializeComponent();
        }

        private async void DealButton_Click(object sender, EventArgs e)
        {
            DealerHandValue = 0;
            PlayerHandOneValue = 0;
            PlayerHandOneAces = 0;
            DealerHandAces = 0;
            PlayerHandTwoActive = false;
            PlayerHandTwoAces = 0;
            PlayerHandOneActive = true;

            if (BetAmount == 0)
            {
                WinOrLossLabel.Text = "Bet Amount is 0.  Please enter a bet amount";
                WinOrLossLabel.Visible = true;
                return;
            }
            if (BetAmount > BankRoll)
            {
                WinOrLossLabel.Text = "Bet is greater than bank roll.  Please enter a valid bet amount";
                WinOrLossLabel.Visible = true;
                return;
            }
            if (remainingCards < 21)
            {
                GetDeckIdFromAPI();
            }
            ResetHand();
            BankRoll -= BetAmount;
            string bankroll_string = "BankRoll Amount: " + BankRoll.ToString();
            BankRollLabel.Text = bankroll_string;

            ChipOne.Enabled = false;
            ChipFive.Enabled = false;
            ChipTen.Enabled = false;
            ChipTwentyFive.Enabled = false;
            ClearBet.Enabled = false;
            HandOneHit.Enabled = true;
            HandOneDouble.Enabled = true;
            HandOneStand.Enabled = true;
            PlayerHandOneActive = true;

            var backImage = "https://www.deckofcardsapi.com/static/img/back.png";

            CardResponse playerCard1 = await drawCard();
            CardResponse playerCard2 = await drawCard();
            CardResponse dealerCard1 = await drawCard();
            CardResponse dealerCard2 = await drawCard();

            var card1 = playerCard1.cards.First();
            HandOneCardOne.Load(card1.image);
            PlayerHandOneValue += GetValueFromCard(card1.value);

            if (GetValueFromCard(card1.value) == 11)
            {
                PlayerHandOneAces += 1;
            }

            var card2 = playerCard2.cards.First();
            HandOneCardTwo.Load(card2.image);
            PlayerHandOneValue += GetValueFromCard(card2.value);

            if (GetValueFromCard(card2.value) == 11)
            {
                PlayerHandOneAces += 1;
            }

            var card3 = dealerCard1.cards.First();
            DealerHandCardOne.Load(card3.image);
            DealerHandCardTwo.Load(backImage);
            DealerHandValue += GetValueFromCard(card3.value);

            if (GetValueFromCard(card3.value) == 11)
            {
                DealerHandAces += 1;
            }

            var card4 = dealerCard2.cards.First();
            HiddenDealerHandValue = GetValueFromCard(card4.value);
            HiddenDealderHandUrl = card4.image;

            if (GetValueFromCard(card4.value) == 11)
            {
                DealerHandAces += 1;
            }

            HandOneCardTwo.Visible = true;
            DealButton.Enabled = false;

            if (GetValueFromCard(card1.value) == GetValueFromCard(card2.value))
            {
                HandOneSplit.Visible = true;
                HandOneSplit.Enabled = true;
            }
            else if (PlayerHandOneValue == 21)
            {
                WinOrLossLabel.Text = "BlackJack!!!";
                WinOrLossLabel.Visible = true;
                BankRoll += BetAmount;
                int blackjack = (int)Math.Round(BetAmount * 1.33);
                BankRoll += blackjack;
                DealButton.Enabled = true;
                ChipOne.Enabled = true;
                ChipFive.Enabled = true;
                ChipTen.Enabled = true;
                ChipTwentyFive.Enabled = true;
                HandOneHit.Enabled = false;
                HandOneStand.Enabled = false;
                HandOneDouble.Enabled = false;
                HandOneSplit.Enabled = false;
                BetAmount = 0;
                DealerHandValue = 0;

            }
            else if (GetValueFromCard(card3.value) >= 10)
            {
                if (GetValueFromCard(card3.value) + GetValueFromCard(card4.value) == 21)
                {
                    WinOrLossLabel.Text = "Dealer Blackjack: ";
                    DealerHandCardTwo.Load(card4.image);
                    WinOrLossLabel.Visible = true;
                    BetAmount = 0;
                    DealerHandValue = 0;
                    DealButton.Enabled = true;
                    HandOneHit.Enabled = false;
                    HandOneDouble.Enabled = false;
                    HandOneSplit.Enabled = false;
                    HandOneStand.Enabled = false;
                    ChipOne.Enabled = true;
                    ChipFive.Enabled = true;
                    ChipTen.Enabled = true;
                    ChipTwentyFive.Enabled = true;

                }
            }

            TestingLabelTextUpdate();

        }


        private void HandTwoStand_Click(object sender, EventArgs e)
        {
            //PlayerHandTwoActive = false;
            DealersTurnLogic();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            string bet_string = "bet amount: " + BetAmount.ToString();
            BetLabel.Text = bet_string;

            string bankroll_string = "BankRoll Amount: " + BankRoll.ToString();
            BankRollLabel.Text = bankroll_string;
            GetDeckIdFromAPI();

        }

        private void ChipOne_Click(object sender, EventArgs e)
        {
            UpdateBetAndBankRoll(1);
        }

        private void ChipFive_Click(object sender, EventArgs e)
        {
            UpdateBetAndBankRoll(5);

        }

        private void ChipTen_Click(object sender, EventArgs e)
        {
            UpdateBetAndBankRoll(10);
        }

        private void ChipTwentyFive_Click(object sender, EventArgs e)
        {
            UpdateBetAndBankRoll(25);
        }

        private void UpdateBetAndBankRoll(int Bet)
        {
            BetAmount += Bet;
            string bet_string = "bet amount: " + BetAmount.ToString();
            BetLabel.Text = bet_string;
            ClearBet.Enabled = true;

        }

        private void ClearBet_Click(object sender, EventArgs e)
        {
            BetAmount = 0;
            string bet_string = "bet amount: " + BetAmount.ToString();
            BetLabel.Text = bet_string;
        }

        private async void GetDeckIdFromAPI()
        {
            string url = "https://www.deckofcardsapi.com/api/deck/new/shuffle/?deck_count=6";

            HttpClient client = new HttpClient();

            // Put the following code where you want to initialize the class
            // It can be the static constructor or a one-time initializer
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();

            using HttpResponseMessage response = await client.GetAsync("");
            response.EnsureSuccessStatusCode();


            var jsonResponse = await response.Content.ReadFromJsonAsync<Shuffle>();


            if (!jsonResponse.success || jsonResponse is null)
            {

                string message = "Unable to retrieve a deck from API.  Please close out window and try again.";
                string title = "Close Window";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result = MessageBox.Show(message, title, buttons);
                if (result == DialogResult.Yes)
                {
                    this.Close();
                }


            }
            else
            {
                deckId = jsonResponse.deck_id;
                remainingCards = jsonResponse.remaining;
            }


        }

        public async Task<CardResponse> drawCard()
        {
            string url = string.Format("https://www.deckofcardsapi.com/api/deck/{0}/draw/?count=1", deckId);

            HttpClient client = new HttpClient();

            // Put the following code where you want to initialize the class
            // It can be the static constructor or a one-time initializer
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();

            using HttpResponseMessage response = await client.GetAsync("");
            response.EnsureSuccessStatusCode();
            remainingCards -= 1;


            var jsonResponse = await response.Content.ReadFromJsonAsync<CardResponse>();

            return jsonResponse;

        }

        public static int GetValueFromCard(string value)
        {
            if (value == "KING")
            {
                return 10;
            }
            if (value == "QUEEN")
            {
                return 10;
            }
            if (value == "JACK")
            {
                return 10;

            }
            if (value == "ACE")
            {
                return 11;
            }
            else
            {

                return int.Parse(value);
            }
        }

        private async void HandOneStand_Click(object sender, EventArgs e)
        {
            if (PlayerHandTwoActive == false)
            {
                DealersTurnLogic();
            }
            HandTwoDouble.Enabled = true;
            HandTwoHit.Enabled = true;
            HandTwoStand.Enabled = true;
            HandOneHit.Enabled = false;
            HandOneSplit.Enabled = false;
            HandOneDouble.Enabled = false;
        }

        private async void DealersTurnLogic()
        {
            HandOneHit.Enabled = false;
            HandOneDouble.Enabled = false;
            HandOneSplit.Enabled = false;
            HandTwoHit.Enabled = false;
            HandTwoStand.Enabled = false;
            HandTwoDouble.Enabled = false;

            bool handSevenUsed = false;

            DealerHandCardTwo.Load(HiddenDealderHandUrl);
            DealerHandValue += HiddenDealerHandValue;

            while (DealerHandValue < 17 && !handSevenUsed)
            {
                CardResponse nextDealerCard = await drawCard();
                var card = nextDealerCard.cards.First();


                if (GetValueFromCard(card.value) == 11 && DealerHandValue > 10)
                {
                    card.value = "1";

                }
                DealerHandValue += GetValueFromCard(card.value);
                if (DealerHandValue > 21 && DealerHandAces > 0)
                {
                    DealerHandAces -= 1;
                    DealerHandValue -= 10;
                }

                Thread.Sleep(1000);
                if (!DealerHandCardThree.Enabled)
                {

                    DealerHandCardThree.Load(card.image);
                    Thread.Sleep(1000);
                    DealerHandCardThree.Enabled = true;
                    DealerHandCardThree.Visible = true;
                    //DealerHandValue += GetValueFromCard(card.value);
                }
                else if (!DealerHandCardFour.Enabled)
                {

                    DealerHandCardFour.Load(card.image);
                    Thread.Sleep(1000);
                    DealerHandCardFour.Enabled = true;
                    DealerHandCardFour.Visible = true;
                    //DealerHandValue += GetValueFromCard(card.value);
                }
                else if (!DealerHandCardFive.Enabled)
                {

                    DealerHandCardFive.Load(card.image);
                    Thread.Sleep(1000);
                    DealerHandCardFive.Enabled = true;
                    DealerHandCardFive.Visible = true;
                    //DealerHandValue += GetValueFromCard(card.value);

                }

                else if (!DealerHandCardSix.Enabled)
                {

                    DealerHandCardSix.Load(card.image);
                    Thread.Sleep(1000);
                    DealerHandCardSix.Enabled = true;
                    DealerHandCardSix.Visible = true;
                    //DealerHandValue += GetValueFromCard(card.value);

                }

                else if (!DealerHandCardSeven.Enabled)
                {

                    DealerHandCardSeven.Load(card.image);
                    Thread.Sleep(1000);
                    DealerHandCardSeven.Enabled = true;
                    DealerHandCardSeven.Visible = true;
                    //#DealerHandValue += GetValueFromCard(card.value);
                    handSevenUsed = true;
                    break;

                }
                TestingLabelTextUpdate();
                HandTwoLabelTextUpdate();
            }
            if (!PlayerHandTwoActive)
            {
                WinOrLossHandOne();
                HandOneDouble.Enabled = false;
                HandOneStand.Enabled = false;
                HandOneHit.Enabled = false;
                HandOneSplit.Enabled = false;
                HandOneSplit.Visible = false;
                HandTwoDouble.Enabled = false;
                HandTwoHit.Enabled = false;
                HandTwoStand.Enabled = false;
                return;

            }

            WinOrLossHandOne();
            WinOrLossHandTwo();
            HandOneDouble.Enabled = false;
            HandOneStand.Enabled = false;
            HandOneHit.Enabled = false;
            HandOneSplit.Enabled = false;
            HandOneSplit.Visible = false;
            HandTwoDouble.Enabled = false;
            HandTwoHit.Enabled = false;
            HandTwoStand.Enabled = false;
        }

        public void WinOrLossHandOne()
        {
            if (PlayerHandOneValue > DealerHandValue)
            {
                WinOrLossLabel.Text = String.Format("Player Wins! Player value {0} - Dealer value {1}", PlayerHandOneValue, DealerHandValue);
                BankRoll += BetAmount * 2;
            }
            else if (DealerHandValue > 21)
            {
                WinOrLossLabel.Text = String.Format("Dealer Busts!  Player wins {0}", DealerHandValue);
                BankRoll += BetAmount * 2;
            }
            else if (DealerHandValue > PlayerHandOneValue)
            {
                WinOrLossLabel.Text = String.Format("Dealder wins.  player value {0} - Dealer value {1}", PlayerHandOneValue, DealerHandValue);
            }
            else if (DealerHandValue == PlayerHandOneValue)
            {
                WinOrLossLabel.Text = String.Format("Push.");
                BankRoll += BetAmount;
            }
            WinOrLossLabel.Visible = true;
            string bankroll_string = "BankRoll Amount: " + BankRoll.ToString();
            BankRollLabel.Text = bankroll_string;
            BetAmount = 0;

            string bet_string = "bet amount: " + BetAmount.ToString();
            BetLabel.Text = bet_string;

            //PlayerHandOneValue = 0;


            if (!PlayerHandTwoActive)
            {
                //DealerHandValue = 0;
                DealButton.Enabled = true;
                ChipOne.Enabled = true;
                ChipFive.Enabled = true;
                ChipTen.Enabled = true;
                ChipTwentyFive.Enabled = true;

            }

        }

        public void WinOrLossHandTwo()
        {
            if (PlayerHandTwoValue > DealerHandValue)
            {
                HandTwoWLLabel.Text = String.Format("Player Wins! Player value {0} - Dealer value {1}", PlayerHandOneValue, DealerHandValue);
                BankRoll += HandTwoBetAmount * 2;
            }
            else if (DealerHandValue > 21)
            {
                HandTwoWLLabel.Text = String.Format("Dealer Busts!  Player wins {0}", DealerHandValue);
                BankRoll += HandTwoBetAmount * 2;
            }
            else if (DealerHandValue > PlayerHandTwoValue)
            {
                HandTwoWLLabel.Text = String.Format("Dealder wins.  player value {0} - Dealer value {1}", PlayerHandOneValue, DealerHandValue);
            }
            else if (DealerHandValue == PlayerHandTwoValue)
            {
                HandTwoWLLabel.Text = String.Format("Push.");
                BankRoll += HandTwoBetAmount;
            }
            HandTwoWLLabel.Visible = true;
            string bankroll_string = "BankRoll Amount: " + BankRoll.ToString();
            BankRollLabel.Text = bankroll_string;
            HandTwoBetAmount = 0;

            string bet_string = "bet amount: " + BetAmount.ToString();
            BetLabel.Text = bet_string;

            //PlayerHandOneValue = 0;
            //PlayerHandTwoValue = 0;



            DealerHandValue = 0;
            DealButton.Enabled = true;
            ChipOne.Enabled = true;
            ChipFive.Enabled = true;
            ChipTen.Enabled = true;
            ChipTwentyFive.Enabled = true;


        }

        private async void HandOneHit_Click(object sender, EventArgs e)
        {
            CardResponse nextCard = await drawCard();
            var card = nextCard.cards.First();


            if (GetValueFromCard(card.value) == 11)
            {
                PlayerHandOneAces += 1;
            }

            PlayerHandOneValue += GetValueFromCard(card.value);

            if (HandOneCardThree.Visible == false)
            {
                HandOneCardThree.Load(card.image);
                HandOneCardThree.Visible = true;
            }
            else if (HandOneCardFour.Visible == false)
            {
                HandOneCardFour.Load(card.image);
                HandOneCardFour.Visible = true;
            }
            else if (HandOneCardFive.Visible == false)
            {
                HandOneCardFive.Load(card.image);
                HandOneCardFive.Visible = true;

            }
            else if (HandOneCardSix.Visible == false)
            {
                HandOneCardSix.Load(card.image);
                HandOneCardSix.Visible = true;
            }
            else if (HandOneCardSeven.Visible == false)
            {
                HandOneCardSeven.Load(card.image);
                HandOneCardSeven.Visible = true;
            }

            if (PlayerHandOneValue > 21 && PlayerHandOneAces > 0)
            {
                PlayerHandOneValue -= 10;
            }

            if (PlayerHandOneValue > 21 && PlayerHandOneAces == 0)
            {
                WinOrLossLabel.Text = "Player Busts.";
                WinOrLossLabel.Visible = true;
                ChipOne.Enabled = true;
                ChipFive.Enabled = true;
                ChipTen.Enabled = true;
                ChipTwentyFive.Enabled = true;
                HandOneHit.Enabled = false;
                HandOneSplit.Enabled = false;
                HandOneDouble.Enabled = false;
                HandOneStand.Enabled = false;
                DealButton.Enabled = true;
                ClearBet.Enabled = true;
                BetAmount = 0;
                string bet_string = "bet amount: " + BetAmount.ToString();
                BetLabel.Text = bet_string;
                string bankroll_string = "BankRoll Amount: " + BankRoll.ToString();
                BankRollLabel.Text = bankroll_string;
                if (PlayerHandTwoActive)
                {
                    HandTwoHit.Enabled = true;
                    HandTwoStand.Enabled = true;
                    HandTwoDouble.Enabled = true;

                }

            }

            TestingLabelTextUpdate();


        }

        private void ResetHand()
        {
            HandOneHit.Enabled = true;
            HandOneDouble.Enabled = true;
            HandOneStand.Enabled = true;
            HandOneSplit.Enabled = false;
            HandOneSplit.Visible = false;
            DealButton.Enabled = false;
            DealerHandCardThree.Visible = false;
            DealerHandCardFour.Visible = false;
            DealerHandCardFive.Visible = false;
            DealerHandCardThree.Enabled = false;
            DealerHandCardFour.Enabled = false;
            DealerHandCardFive.Enabled = false;
            DealerHandCardSix.Enabled = false;
            DealerHandCardSix.Visible = false;
            DealerHandCardSeven.Enabled = false;
            DealerHandCardSeven.Visible = false;
            HandTwoCardOne.Enabled = false;
            HandTwoCardOne.Visible = false;
            HandTwoCardTwo.Enabled = false;
            HandTwoCardTwo.Visible = false;
            HandTwoCardThree.Enabled = false;
            HandTwoCardThree.Visible = false;
            HandTwoCardFour.Enabled = false;
            HandTwoCardFour.Visible = false;
            HandTwoCardFive.Enabled = false;
            HandTwoCardFive.Visible = false;
            HandTwoCardSix.Enabled = false;
            HandTwoCardSix.Visible = false;
            HandTwoCardSeven.Enabled = false;
            HandTwoCardSeven.Visible = false;
            HandOneCardThree.Enabled = false;
            HandOneCardThree.Visible = false;
            HandOneCardFour.Enabled = false;
            HandOneCardFour.Visible = false;
            HandOneCardFive.Enabled = false;
            HandOneCardFive.Visible = false;
            HandOneCardSix.Enabled = false;
            HandOneCardSix.Visible = false;
            HandOneCardSeven.Enabled = false;
            HandOneCardSeven.Visible = false;
            WinOrLossLabel.Visible = false;

        }

        private async void HandOneDouble_Click(object sender, EventArgs e)
        {
            if (BetAmount * 2 > BankRoll)
            {
                return;
            }
            BankRoll -= BetAmount;
            BetAmount += BetAmount;
            CardResponse nextCard = await drawCard();
            var card = nextCard.cards.First();
            string bet_string = "bet amount: " + BetAmount.ToString();
            BetLabel.Text = bet_string;
            string bankroll_string = "BankRoll Amount: " + BankRoll.ToString();
            BankRollLabel.Text = bankroll_string;

            HandOneCardThree.Load(card.image);
            HandOneCardThree.Visible = true;

            PlayerHandOneValue += GetValueFromCard(card.value);
            if (GetValueFromCard(card.value) == 11)
            {
                PlayerHandOneAces += 1;
            }

            if (PlayerHandOneValue > 21 && PlayerHandOneAces == 0)
            {
                WinOrLossLabel.Text = "Player Busts.";
                WinOrLossLabel.Visible = true;
                ChipOne.Enabled = true;
                ChipFive.Enabled = true;
                ChipTen.Enabled = true;
                ChipTwentyFive.Enabled = true;
                HandOneHit.Enabled = false;
                HandOneSplit.Enabled = false;
                HandOneDouble.Enabled = false;
                HandOneStand.Enabled = false;
                DealButton.Enabled = true;
                ClearBet.Enabled = true;
                BetAmount = 0;
                bet_string = "bet amount: " + BetAmount.ToString();
                BetLabel.Text = bet_string;
                bankroll_string = "BankRoll Amount: " + BankRoll.ToString();
                BankRollLabel.Text = bankroll_string;
                return;
            }
            else if (PlayerHandOneValue > 21 && PlayerHandOneAces > 0)
            {
                PlayerHandOneValue -= 10;
                PlayerHandOneAces -= 1;
            }
            TestingLabelTextUpdate();
            if (PlayerHandTwoActive)
            {
                HandTwoHit.Enabled = true;
                HandTwoStand.Enabled = true;
                HandTwoDouble.Enabled = true;
                return;
            }
            DealersTurnLogic();

        }

        private void TestingLabelTextUpdate()
        {
            string TestingString = string.Format("Player Hand One valuer: {0}, Player Aces:{1} Dealer Value {2}, Dealer Aces {3}. HandOneActive {4}. Cards Left{5}", PlayerHandOneValue, PlayerHandOneAces, DealerHandValue, DealerHandAces, PlayerHandOneActive, remainingCards);
            TestingLabel.Text = TestingString;
        }

        private void HandTwoLabelTextUpdate()
        {
            string TestingString = string.Format("Player Hand One valuer: {0}, Player Aces:{1} Dealer Value {2}, Dealer Aces {3}. HandOneActive {4}", PlayerHandTwoValue, PlayerHandTwoAces, DealerHandValue, DealerHandAces, PlayerHandTwoActive);
            HandTwoTestingLabel.Text = TestingString;
        }

        private async void HandOneSplit_Click(object sender, EventArgs e)
        {
            var BetTwoAmount = BetAmount;
            if (BetTwoAmount > BankRoll)
            {
                return;
            }
            PlayerHandTwoActive = true;
            HandTwoCardOne.Image = HandOneCardTwo.Image;
            HandTwoCardOne.Visible = true;
            HandTwoCardTwo.Visible = true;
            HandOneCardTwo.Visible = false;
            HandTwoBetAmount = BetTwoAmount;
            BankRoll -= BetTwoAmount;

            var HandVal = PlayerHandOneValue / 2;
            PlayerHandOneValue -= HandVal;
            PlayerHandTwoValue += HandVal;

            CardResponse playerCard1 = await drawCard();
            CardResponse playerCard2 = await drawCard();

            var card1 = playerCard1.cards.First();
            HandOneCardTwo.Load(card1.image);
            PlayerHandOneValue += GetValueFromCard(card1.value);
            HandOneCardTwo.Visible = true;

            if (GetValueFromCard(card1.value) == 11)
            {
                PlayerHandOneAces += 1;
            }

            var card2 = playerCard2.cards.First();
            HandTwoCardTwo.Load(card2.image);
            PlayerHandTwoValue += GetValueFromCard(card2.value);

            if (GetValueFromCard(card2.value) == 11)
            {
                PlayerHandOneAces += 1;
            }

            if (PlayerHandOneValue == 21)
            {
                WinOrLossLabel.Text = "BlackJack!!! Please hit stand to continue";
                WinOrLossLabel.Visible = true;
                BankRoll += BetAmount;
                int blackjack = (int)Math.Round(BetAmount * 1.33);
                BankRoll += blackjack;
                HandOneHit.Enabled = false;
                HandOneDouble.Enabled = false;
            }
            if (PlayerHandTwoValue == 21)
            {
                WinOrLossLabel.Text = "BlackJack!!! Pleae hit stand to continue";
                WinOrLossLabel.Visible = true;
                BankRoll += BetTwoAmount;
                int blackjack = (int)Math.Round(BetTwoAmount * 1.33);
                BankRoll += blackjack;
                BetTwoAmount = 0;

            }

            TestingLabelTextUpdate();
            HandTwoLabelTextUpdate();
            HandOneSplit.Enabled = false;
            HandTwoHit.Enabled = false;
            HandTwoDouble.Enabled = false;
            HandTwoStand.Enabled = false;

        }

        private void BetLabel_Click(object sender, EventArgs e)
        {

        }

        private void TestingLabel_Click(object sender, EventArgs e)
        {

        }

        private async void HandTwoHit_Click(object sender, EventArgs e)
        {
            CardResponse nextCard = await drawCard();
            var card = nextCard.cards.First();


            if (GetValueFromCard(card.value) == 11)
            {
                PlayerHandTwoAces += 1;
            }

            PlayerHandTwoValue += GetValueFromCard(card.value);

            if (HandTwoCardThree.Visible == false)
            {
                HandTwoCardThree.Load(card.image);
                HandTwoCardThree.Visible = true;
            }
            else if (HandTwoCardFour.Visible == false)
            {
                HandTwoCardFour.Load(card.image);
                HandTwoCardFour.Visible = true;
            }
            else if (HandTwoCardFive.Visible == false)
            {
                HandTwoCardFive.Load(card.image);
                HandTwoCardFive.Visible = true;

            }
            else if (HandTwoCardSix.Visible == false)
            {
                HandTwoCardSix.Load(card.image);
                HandTwoCardSix.Visible = true;
            }
            else if (HandTwoCardSeven.Visible == false)
            {
                HandTwoCardSeven.Load(card.image);
                HandTwoCardSeven.Visible = true;
            }

            if (PlayerHandTwoValue > 21 && PlayerHandTwoAces > 0)
            {
                PlayerHandTwoValue -= 10;
            }

            if (PlayerHandTwoValue > 21 && PlayerHandTwoAces == 0)
            {
                WinOrLossLabel.Text = "Player Busts.";
                WinOrLossLabel.Visible = true;
                ChipOne.Enabled = true;
                ChipFive.Enabled = true;
                ChipTen.Enabled = true;
                ChipTwentyFive.Enabled = true;
                HandTwoHit.Enabled = false;

                HandTwoDouble.Enabled = false;
                HandTwoStand.Enabled = false;
                DealButton.Enabled = true;
                ClearBet.Enabled = true;
                BetAmount = 0;
                string bet_string = "bet amount: " + BetAmount.ToString();
                BetLabel.Text = bet_string;
                string bankroll_string = "BankRoll Amount: " + BankRoll.ToString();
                BankRollLabel.Text = bankroll_string;

            }

            TestingLabelTextUpdate();


        }

        //Logic needs to be changed to fix double blackjack.
        private async void HandTwoDouble_Click(object sender, EventArgs e)
        {
            CardResponse nextCard = await drawCard();
            var card = nextCard.cards.First();

            HandTwoCardThree.Load(card.image);
            HandTwoCardThree.Visible = true;

            PlayerHandTwoValue += GetValueFromCard(card.value);
            if (GetValueFromCard(card.value) == 11)
            {
                PlayerHandTwoAces += 1;
            }

            if (PlayerHandTwoValue > 21 && PlayerHandTwoAces == 0)
            {
                WinOrLossLabel.Text = "Player Busts.";
                WinOrLossLabel.Visible = true;
                ChipOne.Enabled = true;
                ChipFive.Enabled = true;
                ChipTen.Enabled = true;
                ChipTwentyFive.Enabled = true;
                HandOneHit.Enabled = false;
                HandOneSplit.Enabled = false;
                HandOneDouble.Enabled = false;
                HandOneStand.Enabled = false;
                DealButton.Enabled = true;
                ClearBet.Enabled = true;
                BetAmount = 0;
                string bet_string = "bet amount: " + BetAmount.ToString();
                BetLabel.Text = bet_string;
                string bankroll_string = "BankRoll Amount: " + BankRoll.ToString();
                BankRollLabel.Text = bankroll_string;
                return;
            }
            else if (PlayerHandTwoValue > 21 && PlayerHandTwoAces > 0)
            {
                PlayerHandTwoValue -= 10;
                PlayerHandTwoAces -= 1;
            }
            TestingLabelTextUpdate();
            DealersTurnLogic();

        }
    }
}
