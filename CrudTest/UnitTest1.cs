namespace CrudTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //Arrange
            Arthematics arthematics = new Arthematics();
            int input1 = 10; int input2 = 20;
            int addexpected = 30;
            int subexpected = -10;
            int mulexpected = 200;
            int modexpected = 0;

            //Act
            int addactual = arthematics.Addition(input1, input2);
            int subactual = arthematics.Subtraction(input1, input2);
            int modactual = arthematics.Moodulo(input1, input2);
            int mulactual = arthematics.Multiplication(input1, input2);
            //Assert
            Assert.Equal(addexpected, addactual);
            //Assert.Equal(subexpected, subactual);
            //Assert.Equal(mulexpected, mulactual);
            //Assert.Equal(modexpected, modactual);
        }
    }
}