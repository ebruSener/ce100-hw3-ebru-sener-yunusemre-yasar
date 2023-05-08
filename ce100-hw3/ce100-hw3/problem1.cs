using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ce100_hw3_algo_test;



namespace ce100_hw3_algo_lib
{       
     // A class that represents a node in the Huffman tree
    public class HuffmanNode : IComparable<HuffmanNode>
    {
        public char Symbol { get; set; } // The character represented by this node
        public int Frequency { get; set; } // The frequency of the character in the input
        public HuffmanNode Left { get; set; } // The left child of this node
        public HuffmanNode Right { get; set; } // The right child of this node

        // A constructor that creates a leaf node with a given symbol and frequency
        public HuffmanNode(char symbol, int frequency)
        {
            Symbol = symbol;
            Frequency = frequency;
            Left = null;
            Right = null;
        }

        // A constructor that creates an internal node with a given frequency and two children
        public HuffmanNode(int frequency, HuffmanNode left, HuffmanNode right)
        {
            Symbol = '\0'; // Internal nodes do not have symbols
            Frequency = frequency;
            Left = left;
            Right = right;
        }

        // A method that compares two nodes based on their frequencies
        public int CompareTo(HuffmanNode other)
        {
            return Frequency.CompareTo(other.Frequency);
        }
    }

    // A class that implements Huffman coding for compression and decompression
    public class HuffmanCoder
    {
        private Dictionary<char, string> codes; // A dictionary that maps each character to its binary code
        private HuffmanNode root; // The root of the Huffman tree

        // A constructor that takes an input string and builds the Huffman tree and codes
        public HuffmanCoder(string input)
        {
            codes = new Dictionary<char, string>();
            root = BuildTree(input);
            GenerateCodes(root, "");
        }

        // A method that builds the Huffman tree from the input string
        private HuffmanNode BuildTree(string input)
        {
            // Count the frequencies of each character in the input
            var frequencies = new Dictionary<char, int>();
            foreach (char c in input)
            {
                if (frequencies.ContainsKey(c))
                {
                    frequencies[c]++;
                }
                else
                {
                    frequencies[c] = 1;
                }
            }

            // Create a priority queue of leaf nodes based on their frequencies
            var queue = new PriorityQueue<HuffmanNode, int>();
            foreach (var pair in frequencies)
            {
                queue.Enqueue(new HuffmanNode(pair.Key, pair.Value), pair.Value);
            }

            // Build the tree by repeatedly merging the two nodes with the lowest frequencies
            while (queue.Count > 1)
            {
                var left = queue.Dequeue(); // The node with the lowest frequency
                var right = queue.Dequeue(); // The node with the second lowest frequency
                var parent = new HuffmanNode(left.Frequency + right.Frequency, left, right); // A new internal node with the sum of their frequencies
                queue.Enqueue(parent, parent.Frequency); // Add the new node to the queue
            }
            return queue.Dequeue(); // The remaining node is the root of the tree
        }

        // A method that generates the binary codes for each character by traversing the tree recursively
        private void GenerateCodes(HuffmanNode node, string code)
        {
            if (node == null) return; // Base case: empty node

            if (node.Left == null && node.Right == null) // Leaf node: assign the code to the symbol
            {
                codes[node.Symbol] = code;
            }
            else // Internal node: append 0 or 1 to the code and recurse on the children
            {
                GenerateCodes(node.Left, code + "0");
                GenerateCodes(node.Right, code + "1");
            }
        }

        // A method that compresses a file using the Huffman codes and writes the output to another file
        public void CompressFile(string inputFile, string outputFile)
        {
            using (var reader = new StreamReader(inputFile)) // Open the input file for reading
            using (var writer = new BinaryWriter(File.Create(outputFile))) // Open the output file for writing binary data
            {
                var buffer = new List<bool>(); // A buffer to store the bits of the compressed data

                while (!reader.EndOfStream) // Read until the end of the input file
                {
                    char c = (char)reader.Read(); // Read one character from the input file

                    if (codes.ContainsKey(c)) // If the character// has a code, append the bits of the code to the buffer
                    {
                        foreach (char bit in codes[c])
                        {
                            buffer.Add(bit == '1');
                        }
                    }
                    else // If the character does not have a code, throw an exception
                    {
                        throw new Exception("Invalid character: " + c);
                    }

                    if (buffer.Count >= 8) // If the buffer has at least 8 bits, write one byte to the output file
                    {
                        byte b = 0; // A byte to store the bits
                        for (int i = 0; i < 8; i++) // Loop through the first 8 bits of the buffer
                        {
                            b <<= 1; // Shift the byte to the left by one bit
                            if (buffer[i]) b |= 1; // If the bit is 1, set the least significant bit of the byte to 1
                        }
                        writer.Write(b); // Write the byte to the output file
                        buffer.RemoveRange(0, 8); // Remove the first 8 bits from the buffer
                    }
                }

                if (buffer.Count > 0) // If the buffer still has some bits left, write one more byte to the output file
                {
                    byte b = 0; // A byte to store the bits
                    int i = 0; // An index to loop through the buffer
                    while (i < buffer.Count) // Loop until the end of the buffer
                    {
                        b <<= 1; // Shift the byte to the left by one bit
                        if (buffer[i]) b |= 1; // If the bit is 1, set the least significant bit of the byte to 1
                        i++; // Increment the index
                    }
                    b <<= (8 - i); // Shift the byte to the left by the remaining number of bits to fill up a byte
                    writer.Write(b); // Write the byte to the output file
                }
            }
        }

        // A method that decompresses a file using the Huffman codes and writes the output to another file
        public void DecompressFile(string inputFile, string outputFile)
        {
            using (var reader = new BinaryReader(File.OpenRead(inputFile))) // Open the input file for reading binary data
            using (var writer = new StreamWriter(outputFile)) // Open the output file for writing
            {
                var buffer = new List<bool>(); // A buffer to store the bits of the compressed data

                while (reader.BaseStream.Position < reader.BaseStream.Length) // Read until the end of the input file
                {
                    byte b = reader.ReadByte(); // Read one byte from the input file

                    for (int i = 7; i >= 0; i--) // Loop through each bit of the byte from left to right
                    {
                        bool bit = ((b >> i) & 1) == 1; // Get the value of the bit as a boolean
                        buffer.Add(bit); // Add the bit to the buffer

                        char c = Decode(buffer); // Try to decode a character from the buffer

                        if (c != '\0') // If a character is decoded successfully, write it to the output file and clear the buffer
                        {
                            writer.Write(c);
                            buffer.Clear();
                        }
                    }
                }
            }
        }

        // A method that decodes a character from a list of bits by traversing the tree recursively
        private char Decode(List<bool> bits)
        {
            return Decode(bits, root, 0);
        }

        private char Decode(List<bool> bits, HuffmanNode node, int index)
        {
            if (node == null) return '\0'; // Base case: empty node

            if (node.Left == null && node.Right == null) return node.Symbol; // Leaf node: return its symbol

            if (index >= bits.Count) return '\0'; // Base case: no more bits

            bool bit = bits[index]; // Get one bit from the list

            if (bit) return Decode(bits, node.Right, index + 1); // If it is 1, recurse on right child

            return Decode(bits, node.Left, index + 1); // If it is 0, recurse on left child
        }
    }
}