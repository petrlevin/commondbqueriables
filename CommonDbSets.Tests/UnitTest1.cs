using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using CommonDbSets.Tests.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using CommonDbSets;

namespace CommonDbSets.Tests
{

    public class UnitTest1
    {

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            using (var c = new DataContext())
            {
                if (c.Database.Exists())
                {
                    c.Database.Delete();
                }
                c.Database.Create();
                c.Database.Connection.Close();
            }

        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            using (var c = new DataContext())
            {
                if (!c.Database.Exists())
                {
                    return;
                }

                c.Database.Connection.Close();
            }

        }
        private void InitData(DataContext c)
        {
            var content1 = new Content()
                                           {
                                               Author = "Robert Frost",
                                               Text = @"
Some say the world will end in fire,
Some say in ice.
From what I've tasted of desire
I hold with those who favor fire.
But if it had to perish twice,
I think I know enough of hate
To say that for destruction ice
Is also great
And would suffice.",
                                               Title = "FIRE AND ICE"

                                           };

            var content2 = new Content()
            {
                Author = "Robert Frost",
                Text = @"
Something there is that doesn't love a wall,
That sends the frozen-ground-swell under it,
And spills the upper boulders in the sun;
And makes gaps even two can pass abreast.
The work of hunters is another thing:
I have come after them and made repair
Where they have left not one stone on a stone,
But they would have the rabbit out of hiding,
To please the yelping dogs. The gaps I mean,
No one has seen them made or heard them made,
But at spring mending-time we find them there.
I let my neighbour know beyond the hill;
And on a day we meet to walk the line
And set the wall between us once again.
We keep the wall between us as we go.
To each the boulders that have fallen to each.
And some are loaves and some so nearly balls
We have to use a spell to make them balance;
'Stay where you are until our backs are turned!'
We wear our fingers rough with handling them.
Oh, just another kind of out-door game,
One on a side. It comes to little more:
There where it is we do not need the wall:
He is all pine and I am apple orchard.
My apple trees will never get across
And eat the cones under his pines, I tell him
He only says, 'Good fences make good neighbours.'
Spring is the mischief in me, and I wonder
If I could put a notion in his head:
' Why do they make good neighbours? Isn't i'
Where there are cows? But here there are no cows.
Before I built a wall I'd ask to know
What I was walling in or walling out,
And to whom I was like to give offence
Something there is that doesn't love a wall,
That wants it down.' I could say 'Elves' to him,
But it's not elves exacdy, and I'd rather
He said it for himself. I see him there
Bringing a stone grasped firmly by the top
In each hand, like an old-stone savage armed.
He moves in darkness as it seems to me,
Not of woods only and the shade of trees.
He will not go behind his father's saying,
And he likes having thought of it so well
He says again, 'Good fences make good neighbours.",
                Title = "MENDING WALL"

            };
            var cd1 = new ContentDocument()
                         {
                             Content = content1,
                             Date = DateTime.Now,
                             Number = "11234"
                         };
            var cd2 = new ContentDocument()
            {
                Content = content2,
                Date = DateTime.Now,
                Number = "78906"
            };

            var td1 = new TextDocument()
                         {
                             Content = @"
When I see birches bend to left and right
Across the lines of straighter darker trees,
I like to think some boy's been swinging them.
But swinging doesn't bend them down to stay.
Ice-storms do that...
"
                             ,
                             Date = DateTime.Now,
                             Number = "765490"

                         };

            var td2 = new TextDocument()
            {
                Content = @"
He learned all there was
To learn about not launching out too soon
And so not carrying the tree away
Clear to the ground...
"
                ,
                Date = DateTime.Now,
                Number = "790"

            };





            c.Contents.Add(content1);
            c.Contents.Add(content2);
            c.ContentDocument.Add(cd1);
            c.ContentDocument.Add(cd2);
            c.TextDocument.Add(td1);
            c.TextDocument.Add(td2);
            c.SaveChanges();

        }

        [Test]
        public void TestMethod1()
        {

            var i = 9;
    

            using (var c = new DataContext())
            {

               InitData(c);

                var q = c.TextDocument;

                var t1 = c.Set<SimpleDocument, TextDocument>().Select(td => new { type = "", td.Number, td.Date, td.Id });
                var t2 = c.Set<SimpleDocument, ContentDocument>().Select(td => new { type = "", td.Number, td.Date, td.Id });


                
               var result0 = c.Set<ITextDocument, TextDocument>().Where(sd => sd.Number.Contains("u")).Where(sd=>sd.Id>0);
               // result = c.Set<ITextDocument, TextDocument>().Where(sd => sd.Text.Contains("u"));
               result0.ToList();

               var result1 = SearchSimpleDocument(c.Set<SimpleDocument, TextDocument>(), sd => sd.Number.Contains("7"));
               result1 = SearchSimpleDocument(c.Set<SimpleDocument, TextDocument>(), sd => sd.Number.Contains("79"));
               var result2 = SearchSimpleDocument(c.Set<SimpleDocument, ContentDocument>(),
                                                  sd => sd.Number.Contains("7"));
               var result3 = SearchSimpleDocument(c.Set<SimpleDocument, TextDocument>(), sd => sd.Number.Contains("7"));

               result3 = result3.Where(sd => sd.Number.Contains("4"));

                

                var list = result3.ToList();
            }

        }

        public IQueryable<TBase> SearchSimpleDocument<TBase>(IQueryable<TBase> queryable, Expression<Func<TBase, Boolean>> where)
        {
            return queryable.Where(where);
        }
    }
}
