using Xunit;
using Npgsql;
using PgQuery;

namespace GeneralUnitTest
{
    public class ParameterBinderTest
    {
        [Fact]
        public void AddTest()
        {
            ParameterBinder binder = new ParameterBinder();
            object[] values = new object[]
            {
                0, 1, -1, 2.5, 2.5f, true, false, 'c', "string"
            };

            int expect = 0;
            foreach (object value in values)
            {
                Assert.Equal(++expect, binder.Add(value));
            }
        }

        [Fact]
        public void GetTest()
        {
            ParameterBinder binder = new ParameterBinder();
            binder.Add(-1);
            binder.Add(2.5);
            binder.Add(true);
            binder.Add(false);
            binder.Add('c');
            binder.Add("string");
            binder.SetCustom("name", "Smith");
            binder.SetCustom("age", 36);

            Assert.Equal(-1, binder.GetAutoValue(1));
            Assert.True((bool)binder.GetAutoValue(3));
            Assert.Equal("string", binder.GetAutoValue(6));
            Assert.Equal("Smith", binder.GetCustomValue("name"));
            Assert.Equal(36, binder.GetCustomValue("age"));
        }

        [Fact]
        public void ApplyTest()
        {
            ParameterBinder binder = new ParameterBinder();
            binder.Add(50.43);
            binder.Add("string");
            binder.SetCustom("name", "Matthew");
            binder.SetCustom("age", 25);

            NpgsqlCommand command = new NpgsqlCommand();
            binder.Apply(command);
            foreach (NpgsqlParameter param in command.Parameters)
            {
                switch (param.ParameterName)
                {
                    case "1":
                        Assert.Equal(50.43, param.Value); break;
                    case "2":
                        Assert.Equal("string", param.Value); break;
                    case "name":
                        Assert.Equal("Matthew", param.Value); break;
                    case "age":
                        Assert.Equal(25, param.Value); break;
                    default:
                        Assert.True(false); break;
                }
            }
        }
    }
}
